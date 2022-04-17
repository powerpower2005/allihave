using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

//Nuget으로 패키지 다운받음.
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

//유니티에서 실행
using UnityEngine;


//구글 API를 사용하기 위해서는 구글 클라우드 플랫폼에서 프로젝트를 만들어야 활성화가 가능함.

//1. API 받아오기
//2.Client ID : GOOGLE에서 알아서 받자 -> JSON 파일로 가지고 있어야함
//3.서비스계정


//Want to Read GoogleSpreadSheet and transform to json type
//-> 구글스프레드시트에 정리한 내용을 json파일로 만들어 이용함
public partial class Builder : MonoBehaviour
{
    static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
    static string ApplicationName = "Google Sheets Parser";

    public string fileName = "(임의 이름)"; // StaticData  이름
    public string version="(임의 버전)"; // 해당 버전의 StaticData Update


    [SerializeField] public bool hasUpdated; //스프레드시트에서 가져왔는지 체크하는 플래그


    //실제로 Data가 담길 클래스
    public DataWrapper dataWrapper;


    private void Awake()
    {
        //Build -> 구글스프레드시트에서 가져오는 작업
        if (!hasUpdated)
        {
            Build().Forget();
            hasUpdated = !hasUpdated;
        }
        //SaveWrite -> 가져온 것을 JSON으로 만드는 작업
        else
        {
            SaveWrite().Forget();
        }

    }


    /// 서버 정보를 가져옴
    async UniTaskVoid Load() {
        await Random.Server.LoadConfig();
        var serverInfo = Random.Server.Config.servers[0];//해당 환경 설정 이후 서버에 있는 Info를 받아옴 -> 임의
        //서버에 있는 info를 이용하여 파일을 받아옴.
        var url = $"{serverInfo.url}/projects/{ProjectCode}/{serverInfo.serverCode}/data/{version}/{fileName}";
        //해당 데이터 업데이트
        dataWrapper = await Random.Get<DataWrapper>(url);
    }

    //서버에 기록
    async UniTaskVoid SaveWrite()
    {
        //로컬에 쓰기(Write)
        var data = Builder.Write("../DataFromSheet.json");
        //서버에쓰기(Save)
        await Random.Server.LoadConfig();

        //Center에 Request 보냄.
        await Random.Center.SaveStaticData(Random.Server.Config.servers[0], version,fileName, data);
        Debug.Log("[WritingSuccess]");
    }


    //실제로 구글 스프레드시트를 가져옴
    static async UniTaskVoid Build()
    {

        UserCredential credential;
        //위에서 다운받은 credential path를 입력한다 -> 2번파일 경로
        using (var stream =
               new FileStream(
                  "client_secret_.........json",
                  FileMode.Open, FileAccess.Read))
        {
            //토큰을 저장함(자동생성)
            // The file token.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            string credPath = "token.json";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
               GoogleClientSecrets.Load(stream).Secrets,
               Scopes,
               "user",
               CancellationToken.None,
               new FileDataStore(credPath, true)).Result;
            Debug.Log("Credential file saved to: " + credPath);
        }

        // Create Google Sheets API service -> 이것을 이용해서 GoogleSpreadSheet 접근 및 작업 가능
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        //Define request parameter -> 우리가 접근할 Sheet와 범위
        string spreadsheetId = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"; //구글 시트(특정 시트 x, 시트 전체 o)
        var sheetsName = GetSheetNameByID(service, spreadsheetId); //해당 Id를 가지는 구글시트 전체를 가져옴.


        string body = "";

        //모든 시트 이름에서
        foreach (var name in sheetsName)
        {
            // .으로 시작하는 Sheet는 무시 ignore file started with '.'(dot)
            var s = name[0];
            if (name.Length > 0 && s == '.') continue;
            body += await Parse(service, spreadsheetId, name); // 모든 Sheet에 대해 수행.
            body += "\n\n";
        }

        var path = "../xxxx/xxxxxx.cs";

        //해당 경로의 파일 이름으로 Data(Stream)를 Text로 내보낼 때 쓰임.
        using (StreamWriter outputFile = new StreamWriter(path))
        {
            //해당 파일에 body를 write함.
            await outputFile.WriteAsync(body);
        }

        Debug.Log("[build success]");
        
    }

    /// 해당Id의 스프레드 시트 전체의 이름을 가져옴.
    static List<string> GetSheetNameByID(SheetsService Service, string spreadsheetId)
    {
        bool includeGridData = false;
        List<string> names = new List<string>();
        SpreadsheetsResource.GetRequest request = Service.Spreadsheets.Get(spreadsheetId);
        request.IncludeGridData = includeGridData;

        Spreadsheet response = request.Execute();
        foreach (var sheet in response.Sheets)
        {
            names.Add(sheet.Properties.Title);
        }

        return names;
    }


    /// 해당 id 시트의 name을 가진 워크시트의 value를 파싱함 (내부 구현은 사용자 정의)
    static async Task<string> Parse(SheetsService service, string spreadsheetId, string name)
    {
        //해당 id의 name을 가지고 있는 Sheet에 대해 접근하는 Request
        SpreadsheetsResource.ValuesResource.GetRequest request =
           service.Spreadsheets.Values.Get(spreadsheetId, name);

        ValueRange response = request.Execute();
        IList<IList<System.Object>> values = response.Values; //행을 뜻함(row)

        string body = "";
        //bool isBody = false;


        //행이 존재해야함.
        if (values != null && values.Count > 0)
        {
            for (var i = 0; i < values.Count; i++)
            {
                var row = values[i];
                if (row.Count == 0) continue;
                if (row[0].ToString().Equals("[END]"))break;
                body += row[0].ToString(); //1번째 열만 body에 추가시킴.(실제로 필요한 것)
                body += "\n";
            }
        }
        else
        {
            Debug.Log("No data found.");
        }

        Debug.Log(body);

        return body;
    }

}
