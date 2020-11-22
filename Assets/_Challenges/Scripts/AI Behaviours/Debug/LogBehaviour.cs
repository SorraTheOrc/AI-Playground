﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MessyCoderCommunity.AI
{
    /// <summary>
    /// The Log Behaviour simply logs a message to the console every tick.
    /// </summary>
    [CreateAssetMenu(fileName = "Log Behaviour", menuName = "Messy AI/Debug/Log")]
    public class LogBehaviour : GenericAiBehaviour<GameObject>
    {
        [SerializeField, Tooltip("The message to display. This can include variables on the chalkboard using `{VARIABLE_NAME}`.")]
        string message = "{agent} says 'Hi'";

        private Regex variableRegex;

        public override void Initialize(GameObject agent, IChalkboard chalkboard)
        {
            base.Initialize(agent, chalkboard);

            variableRegex = new Regex(@"\{([^}]*)\}", RegexOptions.Compiled);
        }

        public override void Tick(IChalkboard chalkboard)
        {
            string expandedMessage = message;
            MatchCollection matches = variableRegex.Matches(expandedMessage);
            for (int i = 0; i < matches.Count; i ++)
            {
                string token = matches[i].Groups[0].Value;
                int index = matches[i].Groups[0].Index;
                string variableName = matches[i].Groups[1].Value;

                UnityEngine.Object unityValue = chalkboard.GetUnity<UnityEngine.Object>(variableName);
                if (unityValue != null)
                {
                    expandedMessage = expandedMessage.Remove(index, token.Length).Insert(index, unityValue.ToString());
                } else
                {
                    System.Object systemValue = chalkboard.GetSystem<System.Object>(variableName);
                    if (systemValue != null)
                    {
                        expandedMessage = expandedMessage.Remove(index, token.Length).Insert(index, systemValue.ToString());
                    } else
                    {
                        expandedMessage = expandedMessage.Remove(index, token.Length).Insert(index,
                            "[Missing or unrecognized type for variable " + token + "]");
                    }
                }
                
            }

            Debug.Log(expandedMessage);
        }
    }
}
