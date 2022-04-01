using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace lkangr
{

    public class Manager : MonoBehaviour
    {
        public CanvasGroup Layer1;
        public CanvasGroup Layer2;

        public Image LED_ON;
        public Image PUMP_ON;

        public Text Temp;
        public Text Humi;

        public Component speedometer;

        // Start is called before the first frame update
        void Start()
        {
         
        }

        private void Update()
        {
            
        }

        public void ChangeToLayer1()
        {
            Layer1.gameObject.SetActive(true);
            Layer2.gameObject.SetActive(false);
        }

        public void ChangeToLayer2()
        {
            Layer1.gameObject.SetActive(false);
            Layer2.gameObject.SetActive(true);
        }

        public void Update_Status(Status_Data _status_data)
        {
            Temp.text = _status_data.TEMP + "°C";
            Humi.text = _status_data.HUMI + "%";

            GetComponent<Speedometer>().updateTemp(float.Parse(_status_data.TEMP));
        }

        public void Update_LED(LED_Data _led_data_r)
        {
            if (_led_data_r.status == "ON")
            {
                LED_ON.gameObject.SetActive(true);
            }
            else
            {
                LED_ON.gameObject.SetActive(false);
            }
        }

        public void Update_LED_Value(LED_Data _led_data_s)
        {
            _led_data_s.device = "LED";
            _led_data_s.status = (LED_ON.gameObject.activeSelf ? "OFF" : "ON");
        }

        public void Update_PUMP(PUMP_Data _pump_data_r)
        {
            if (_pump_data_r.status == "ON")
            {
                PUMP_ON.gameObject.SetActive(true);
            }
            else
            {
                PUMP_ON.gameObject.SetActive(false);
            }
        }

        public void Update_PUMP_Value(PUMP_Data _pump_data_s)
        {
            _pump_data_s.device = "PUMP";
            _pump_data_s.status = (PUMP_ON.gameObject.activeSelf ? "OFF" : "ON");
        }
    }
}