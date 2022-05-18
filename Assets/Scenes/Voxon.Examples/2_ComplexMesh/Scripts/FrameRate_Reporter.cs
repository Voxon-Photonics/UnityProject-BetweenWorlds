using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon.Examples
{
    public class FrameRateReporter : MonoBehaviour {
        private const float StdDevPercent = 0.05f;

        //Declare these in your class
        private int _mFrameCounter;
        private float _mTimeCounter;
        private float _mLastFramerate;
        [FormerlySerializedAs("m_refreshTime")] public float mRefreshTime = 0.5f;

        private List<float> _frames;

        private float _average;
        private float _standardDeviation;

        private bool _stabalised;

        private void Start()
        {
            Reset();
        }

        public void Reset()
        {
            _frames = new List<float>();
            _stabalised = false;
        }

        private void GetStandardDeviation()
        {
            _average = _frames.Average();
            if(_frames.Count < 2)
            {
                _standardDeviation = 0;
                return;

            }

            float sumOfDerivation = 0;
            foreach (float value in _frames)
            {
                sumOfDerivation += (value) * (value);
            }
            float sumOfDerivationAverage = sumOfDerivation / (_frames.Count - 1);
            float newStandardDeviation = Mathf.Sqrt(sumOfDerivationAverage - (_average * _average));
            if(!_stabalised && Mathf.Abs(_standardDeviation - newStandardDeviation) < StdDevPercent)
            {
                Debug.Log("Stablised");
                _stabalised = true;
            }

            _standardDeviation = newStandardDeviation;
        }

        private void Update()
        {
            if (_mTimeCounter < mRefreshTime)
            {
                _mTimeCounter += Time.deltaTime;
                _mFrameCounter++;
            }
            else
            {
                //This code will break if you set your m_refreshTime to 0, which makes no sense.
                _mLastFramerate = _mFrameCounter / _mTimeCounter;
                _mFrameCounter = 0;
                _mTimeCounter = 0.0f;

                _frames.Add(_mLastFramerate);
                GetStandardDeviation();
                if(_stabalised)
                {
                    // Debug.Log(average + ", +/- " + standard_deviation);
                    VXProcess.add_log_line(_average + ", +/- " + _standardDeviation);
                }
                else
                {
                    VXProcess.add_log_line("Stabalising...");
                }
            
            }
        }
    }
}
