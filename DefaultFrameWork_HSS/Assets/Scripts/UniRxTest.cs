using UnityEngine;
using UniRx;
using System.Collections;
using System;

public class UniRxTest : MonoBehaviour
{
    public Subject<int> testSubject = new Subject<int>();
    public IObserver<int> TestObserver => testSubject;



    public IEnumerator Start()
    {
        var wait = new WaitForSeconds(1f);

        //yield return wait;
        //testSubject.OnNext(1000);
        //yield return wait;
        //testSubject.OnNext(10000);
        //yield return wait;
        //testSubject.OnCompleted();

        yield return wait;
        TestObserver.OnNext(1000);
        yield return wait;
        TestObserver.OnCompleted();
    }

}
