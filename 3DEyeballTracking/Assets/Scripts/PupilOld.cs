namespace MyProject.PupilOld
{
    using UnityEngine;
    using System.Collections.Generic;

    public class Pupil : MonoBehaviour
    {
        public Transform iris; // The smaller sphere for the pupil/iris
        private float screenWidth = 1920f; // Adjust according to your data
        private float screenHeight = 1080f;

        private List<Vector2> gazeData = new List<Vector2>(); // To store gaze positions
        private int currentIndex = 0; // Tracks the current data point
        public float playbackSpeed = 1f; // Playback speed multiplier
        private float timer = 0f;

        void Start()
        {
            LoadGazeDataFromCSV();
        }

        void Update()
        {
            if (gazeData.Count == 0) return;

            // Advance timer
            timer += Time.deltaTime * playbackSpeed;

            // If enough time has passed, update the gaze
            if (timer >= 0.1f) // Example: 0.1 seconds per data point
            {
                timer = 0f; // Reset timer
                UpdateGazeFromData();
            }
        }

        private void LoadGazeDataFromCSV()
        {
            TextAsset csvFile = Resources.Load<TextAsset>("FirstData"); // CSV file in Resources folder
            if (csvFile == null)
            {
                Debug.LogError("CSV file not found! Place it in the Resources folder.");
                return;
            }

            string[] lines = csvFile.text.Split('\n');
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue; // Skip empty lines

                string[] values = line.Split(',');
                if (values.Length >= 2)
                {
                    // Parse gaze_X and gaze_Y values
                    if (float.TryParse(values[0], out float gazeX) && float.TryParse(values[1], out float gazeY))
                    {
                        gazeData.Add(new Vector2(gazeX, gazeY));
                    }
                }
            }

            Debug.Log($"Loaded {gazeData.Count} gaze points.");
        }

        private void UpdateGazeFromData()
        {
            if (currentIndex >= gazeData.Count)
            {
                currentIndex = 0; // Loop back to the start if we've reached the end
            }

            Vector2 gazePoint = gazeData[currentIndex];
            UpdateGaze(gazePoint.x, gazePoint.y);
            currentIndex++;
        }

        public void UpdateGaze(float posX, float posY)
        {
            float rotationX = Mathf.Lerp(-30f, 30f, posY / screenHeight);
            float rotationY = Mathf.Lerp(-30f, 30f, posX / screenWidth);
            Debug.Log($"RotationX: {rotationX}, RotationY: {rotationY}");
            iris.localRotation = Quaternion.Euler(rotationX, -rotationY, 0f);
        }
    }
}
