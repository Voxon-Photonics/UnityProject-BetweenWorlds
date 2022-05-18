using UnityEngine;

namespace Voxon.Examples
{
    public class Touched : MonoBehaviour {
        private int _transpose = -4;  // transpose in semitones
        private AudioSource _sound;

        public int position;
        public int sampleRate = 44100;
        public int frequency = 440;
        private AudioClip _myClip;

        // Use this for initialization
        private void Start () {
            if(GetComponent<AudioSource>() == null)
            {
                gameObject.AddComponent<AudioSource>();
            }

            AudioClip myClip = AudioClip.Create("MyTone", 44100, 1, 24500, true, OnAudioRead, OnAudioSetPosition);

            sampleRate = AudioSettings.outputSampleRate;
            _sound = GetComponent<AudioSource>();
            _sound.clip = myClip;
        
            _sound.minDistance = 1;
            _sound.volume = 0.25f;
            _sound.spatialBlend = 0f;
        }
	
        // Update is called once per frame
        void Update () {

            if (gameObject.GetComponent<Renderer>().sharedMaterial.color != Color.white)
            {
                Color tmp = gameObject.GetComponent<Renderer>().sharedMaterial.color;
                if (tmp.r < 1.0)
                {
                    tmp.r += 0.01f;
                }
                if (tmp.b < 1.0)
                {
                    tmp.b += 0.01f;
                }
                if (tmp.g < 1.0)
                {
                    tmp.g += 0.01f;
                }

                _sound.volume = _sound.volume - 0.025f;
                gameObject.GetComponent<Renderer>().sharedMaterial.color = tmp;
            }
            else if (_sound.isPlaying)
            {
                _sound.Stop();
            }

        }

        private void OnCollisionEnter(Collision collision)
        {
            Color col = collision.gameObject.GetComponent<Renderer>().sharedMaterial.color;
            gameObject.GetComponent<Renderer>().sharedMaterial.color = col;

            float note = -1; // invalid value to detect when note is pressed

            if (col == Color.red) note = 0;  // C
            else if (col == Color.green) note = 2;  // D
            else if (col == Color.blue) note = 4;  // E
            else if (col == Color.cyan) note = 5;  // G
            else if (col == Color.magenta) note = 9;  // B
            else if (col == Color.white) note = 11;  // C
            else
            {
                note = 7;
            }

            if (note >= 0)
            {
                _sound.volume = 0.25f;
                _sound.pitch = Mathf.Pow(2, (note + _transpose) / 12.0f);
                _sound.Play();

            }
        }


        void OnAudioRead(float[] data)
        {
            int count = 0;
            while (count < data.Length)
            {
                data[count] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * position / sampleRate));
                position++;
                count++;
            }
        }
        void OnAudioSetPosition(int newPosition)
        {
            position = newPosition;
        }
    }
}
