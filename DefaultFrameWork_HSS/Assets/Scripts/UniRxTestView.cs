using UnityEngine;
using UniRx;
using HSS;
using UnityEngine.UI;
using System;

public class UniRxTestView : MonoBehaviour
{
    public Text uniRxText;

    public void Start()
    {
        // �����ϸ� ���Ŀ� ������ �˷��ִ°ǰ�..  Ȱ�뵵�� �ſ� ���� ����...
        var testSubject = GetComponent<UniRxTest>().testSubject;
        testSubject.Subscribe(x => TestView(x));

        var clickStream = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0));
        clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(250)))
            .Where(x => x.Count >= 2).SubscribeToText(uniRxText, x => $"Double Click / Click Count = {x.Count}");
    }

    private void TestView(int num)
    {
        HSSLog.Log($"num : {num}", LogColor.cyan);
    }
}
