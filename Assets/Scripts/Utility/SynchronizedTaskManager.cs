//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Photon.Pun;

//public class SynchronizedTaskManager
//{
//    public bool IsReadyToContinue { get; private set; }
//    private Queue<Action> taskList;

//    public void AddTask(Action newTask)
//    {
//        if (newTask != null)
//        {
//            taskList.Enqueue(newTask);
//        }
//    }

//    [PunRPC]
//    public void ReadyCheckAndExecute(Action newTask)
//    {
//        if(IsReadyToContinue)
//        {
//            if (GameManager.Instance.offlineMode)
//            {
//                newTask?.Invoke();
//            }
//            else if(PhotonNetwork.IsMasterClient)
//            {
//                IsReadyToContinue = false;
//                newTask();
//                GameManager.Instance.ExecuteFromDistance(newTask);
//            }
//            else
//            {
//                newTask?.Invoke();
//                ExecuteFromDistance

//            }
//        }
//        else
//        {
//            if (newTask != null)
//            {
//                taskList.Enqueue(newTask);
//            }
//        }

//        if (taskList.Count > 0)
//        {
//            if (GameManager.Instance.offlineMode)
//            {
//                task = taskList.Dequeue();
//                ReadyCheckDelegate();
//            }
//            else if (PhotonNetwork.IsMasterClient)
//            {
//                if (isReadyToContinue)
//                {
//                    isReadyToContinue = false;
//                    ReadyCheckDelegate = ReadyCheckDelegateQueue.Dequeue();
//                    ReadyCheckDelegate();                                                               // Is there too much latenty?
//                    photonView.RPC("ReadyCheck", RpcTarget.Others, ReadyCheckDelegate.Method.Name);
//                }
//            }
//            else
//            {
//                ReadyCheckDelegate = ReadyCheckDelegateQueue.Dequeue();
//                ReadyCheckDelegate();
//                photonView.RPC("ResumeReadyCheck", RpcTarget.MasterClient);
//            }
//        }
//    }

//    //photonView.RPC("ExecuteFromDistance", RpcTarget.Others, ReadyCheckDelegate.Method.Name);

//    public void ExecuteFromDistance(Action task)
//    {
//        if(!offlineMode)
//        {
//            photonView.RPC("ExecuteFromDistance", RpcTarget.Others, ReadyCheckDelegate.Method.Name);
//        }
//    }

//    public void ExecuteFromDistance(String taskName)
//    {
//        MethodInfo methodInfo = this.GetType().GetMethod(taskName);
//        if (methodInfo != null)
//            ReadyCheckAndExecute((Action)Delegate.CreateDelegate(typeof(Action), this, methodInfo));
//    }
//}
