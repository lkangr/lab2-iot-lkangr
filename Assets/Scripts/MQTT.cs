/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

/// <summary>
/// Examples for the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// </summary>
namespace lkangr
{
    /// <summary>
    /// Script for testing M2MQTT with a Unity UI
    /// </summary>
    /// 

    public class Status_Data
    {
        public string TEMP { get; set; }
        public string HUMI { get; set; } 
    }

    public class LED_Data
    {
        public string device { get; set; }
        public string status { get; set; }
    }

    public class PUMP_Data
    {
        public string device { get; set; }
        public string status { get; set; }
    }

    public class Data_ss
    {
        public string ss_name { get; set; }
        public string ss_value { get; set; }
    }

    public class MQTT : M2MqttUnityClient
    {
        public InputField BrokenURI;
        public InputField Username;
        public InputField Password;

        public Text error_text;

        public List<string> topics = new List<string>();

        public string msg_received_from_topic_status = "";
        public string msg_received_from_topic_LED = "";
        public string msg_received_from_topic_PUMP = "";

        private List<string> eventMessages = new List<string>();
        [SerializeField]
        public Status_Data _status_data;
        [SerializeField]
        public LED_Data _led_data_r, _led_data_s;
        [SerializeField]
        public PUMP_Data _pump_data_r, _pump_data_s;

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
        }

        public void Push_Connect()
        {
            this.brokerAddress = BrokenURI.text;
            this.mqttUserName = Username.text;
            this.mqttPassword = Password.text;
            error_text.text = "connecting...";
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            error_text.text = "";
            SubscribeTopics();
            GetComponent<Manager>().ChangeToLayer2();
        }

        protected override void SubscribeTopics()
        {

            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }

        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            error_text.text = "Connect Failed! " + errorMessage;
        }

        protected override void OnConnectionLost()
        {
            base.OnDisconnected();
            error_text.text = "Lost connection!";
            GetComponent<Manager>().ChangeToLayer1();
        }

        protected override void Start()
        {

            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            if (topic == topics[0])
                ProcessMessageStatus(msg);
            else if (topic == topics[1])
                ProcessMessageLED(msg);
            else if (topic == topics[2])
                ProcessMessagePUMP(msg);
        }

        private void ProcessMessageStatus(string msg)
        {
            _status_data = JsonConvert.DeserializeObject<Status_Data>(msg);
            msg_received_from_topic_status = msg;
            GetComponent<Manager>().Update_Status(_status_data);
        }

        public void ProcessMessageLED(string msg)
        {
            _led_data_r = JsonConvert.DeserializeObject<LED_Data>(msg);
            msg_received_from_topic_LED = msg;
            GetComponent<Manager>().Update_LED(_led_data_r);
        }

        public void ProcessMessagePUMP(string msg)
        {
            _pump_data_r = JsonConvert.DeserializeObject<PUMP_Data>(msg);
            msg_received_from_topic_PUMP = msg;
            GetComponent<Manager>().Update_PUMP(_pump_data_r);
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {

        }

        public void PublishLED()
        {
            _led_data_s = new LED_Data();
            GetComponent<Manager>().Update_LED_Value(_led_data_s);
            string msg_config = JsonConvert.SerializeObject(_led_data_s);
            client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish LED");
        }

        public void PublishPUMP()
        {
            _pump_data_s = new PUMP_Data();
            GetComponent<Manager>().Update_PUMP_Value(_pump_data_s);
            string msg_config = JsonConvert.SerializeObject(_pump_data_s);
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish PUMP");
        }
    }
}
