using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace cdreyer
{
    public class UILog : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI messageTMRPO;
        [SerializeField] TextMeshProUGUI stackTraceTMRPO;
        [SerializeField] int maxTraceAmount;

        string[] traceMessages;

        public void Init(Log log)
        {
            messageTMRPO.text = log.message;
            traceMessages = log.traceMessages;

            if (maxTraceAmount == 0)
            {
                stackTraceTMRPO.text = "";
                return;
            }

            stackTraceTMRPO.text = traceMessages.LastOrDefault() ?? "";
            int count = 1;
            for (int i = traceMessages.Length - 2; i >= 0; i--)
            {
                if (count >= maxTraceAmount) break;

                TextMeshProUGUI newTrace = Instantiate(stackTraceTMRPO, stackTraceTMRPO.transform.parent);
                newTrace.text = traceMessages[i];

                count++;
            }
        }
    }
}