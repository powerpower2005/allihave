using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;


public class CameraElevatorController
{
    private readonly Camera camera;

    private readonly CameraElevatorModel cameraElevatorModel;

    private bool _isDown;
    private bool _isDownOut;
    private Vector2 _prevPos;
    private float _velocityY;
    private Vector2 downPos;
    private bool isDrag;

//Model 주입으로 디커플링.
    public CameraElevatorController(CameraElevatorModel cameraElevatorModel)
    {
        this.cameraElevatorModel = cameraElevatorModel;
        this.cameraElevatorModel.onChange += UpdateView;

        camera = Camera.main;
        camera.transform.SetParent(cameraElevatorView.transformRoot);
        cameraElevatorView.transformRoot.localPosition = new Vector3(0, 0, -10);
        camera.transform.localPosition = Vector3.zero;

        Process().Forget();
    }

    //해당위치에 UI가 있는지 없는지 
    private bool IsPointerOverUIObject(Vector2 pos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        eventDataCurrentPosition.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        //Ray로 쏴서 Raycast Target 에 걸리는 것이 있는지 -> Canvas 체크
        return results.Count > 0;
    }
    private async UniTaskVoid Process()
    {
        while (true)
        {

            //TargetY가 존재할 때
            if (cameraElevatorModel.IsScrollToTarget)
            {
                //카메라를 이동시킴
                cameraElevatorModel.SetY(Mathf.Lerp(cameraElevatorModel.Y,cameraElevatorModel.TargetY,Time.deltaTime*10f));
                //해당 설정된 Y위치와 일정 간격이하가 되었다면 TargetY를 없앰.
                if (Mathf.Abs(cameraElevatorModel.Y - cameraElevatorModel.TargetY) < 0.3f)
                {
                    cameraElevatorModel.ClearTarget();
                }
            }
                
            //var limit = mainNavModel.SelectTabIndex == -1 ? 120 : Screen.height * G.Constant.UIBottomRate;


            //마우스의 위치와 현재 카메라가 가리키고 있는 Y좌표의 값을 매번 갱신함.
            Vector2 mp = UnityEngine.Input.mousePosition;

            var y = cameraElevatorModel.Y; // 현재 보고있는 카메라의 위치


            //이전에 누르지 않고서 최초로 버튼을 눌렀다면(멀티 터치 방지(두 번 누르는 것을 Elevator에선 허용하지 않음)
            if (!_isDown && UnityEngine.Input.GetMouseButtonDown(0) &&
                !IsPointerOverUIObject(mp))
            {
                //최초로 누른 마우스 위치를 저장
                _isDown = true;
                _isDownOut = false;
                _velocityY = 0;
                isDrag = false;
                downPos = mp;
            }
            //누르기만하고 충분히 움직이지 않고 뗀다면 카메라는 이동하지 않음) 
            if (_isDown && UnityEngine.Input.GetMouseButtonUp(0))
            {
                _isDownOut = false;
                _isDown = false;
            }

            //누른 상태라면 -> 카메라가 이동하는 부분 **
            if (_isDown)
            {

                //누른 상태에서 마우스를 움직이면
                if (!_isDownOut)
                {
                    //(최초로 누른 지점에서와 현재의 포인터의 지점 차이기 일정 이상 커지면 downOut이 True가 됨)
                    if (Vector2.SqrMagnitude(downPos - mp) >= G.Constant.TouchScreenSensitive * Screen.height *
                        G.Constant.TouchScreenSensitive * Screen.height)
                        _isDownOut = true;
                }
                else
                {
                    //_isDownOut이 true가 된 상태라면 카메라를 움직일만큼 마우스 포지션이 바뀌었다는 이야기
                    var d = camera.ScreenToWorldPoint(mp) - camera.ScreenToWorldPoint(_prevPos);
                    y = cameraElevatorModel.Y - d.y; //y의 값을 역으로 빼주어 위로 드래그하면 위치가 낮아지고 아래로 드래그하면 위치가 높아지는

                    //d값이 클수록 -> 짧은 시간 내에 많이 움직였으면 움직이는 속력이 더 큼.
                    _velocityY += d.y * Time.deltaTime * 30f;
                    _velocityY = Mathf.Lerp(_velocityY, 0, Time.deltaTime * 20f);
                    isDrag = true;
                    //1 2 3
                    //1 2 3 4 5 6
                }

                //카메라의 위치를 y로 이동시킴
                //y의 갱신이 누른 상태에서 일정 이상 옮겼을 때만 이루어짐.
                cameraElevatorModel.SetY(y);
            }
            //누른 상태가 아니라면 -> 현 카메라가 가리키는 위치가 MAX보다 높진 않은지, MIN 아래보다 낮진 않은지 체크.
            else
            {
                //만약 현재 가리키는 위치가 정상적인 위치라면
                if (IsClamp(y))
                {
                    //정상적인 위치에서 드래그 True인 상태라면 velocityY에 저장된 속력으로 카메라를 이동시킴.
                    if (isDrag && Mathf.Abs(_velocityY) >= 0.0001f)
                    {
                        y = cameraElevatorModel.Y - _velocityY;
                        _velocityY = Mathf.Lerp(_velocityY, 0, Time.deltaTime * 5f);
                    }

                    cameraElevatorModel.SetY(y);
                }
                else
                {
                    //정상적인 위치가 아니면 카메라를 최저층이나 최고층으로 되돌림 (어차피 최저, 최고 사이만 정상높이)
                    ClampSet(y);
                }
            }

                
            _prevPos = mp;
            await UniTask.Yield(G.ctx.Token);
        }
            
    }


    private bool IsClamp(float y)
    {
        //카메라가 비춰야 하는 최소 높이, 최대 높이 제한.
        var minY = //TODO
        var maxY = //TODO
        return y >= minY && y < maxY;
    }

    private void ClampSet(float y)
    {
        var minY = //TODO
        var maxY = //TODO
        if (y < minY)
            y = Mathf.Lerp(y, minY, Time.deltaTime * 5f);
        else if (y > maxY) y = Mathf.Lerp(y, maxY, Time.deltaTime * 5f);

        //카메라 범위 벗어나면 다시 설정.
        cameraElevatorModel.SetY(y);
    }

    //private void UpdateView()
    //{
    //    cameraElevatorView.transform.position = new Vector2(0,cameraElevatorModel.Y);
    //}
}
