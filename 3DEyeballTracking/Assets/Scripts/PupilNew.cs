namespace MyProject.PupilNew
{
    using UnityEngine;
    using System.Collections.Generic;

    public class Pupil : MonoBehaviour
    {
        // Screen size (adjust based on your display setup)
        private float screenWidth = 1920f;
        private float screenHeight = 1080f;

        private List<Vector3> gazeData = new List<Vector3>(); // To store gaze positions and fixation times
        private int currentIndex = 0; // Tracks the current gaze point
        private float timer = 0f; // Timer to manage fixation time
        private float currentFixationTime = 0f; // Current fixation time in seconds

        public float maxRotationAngle = 30f; // Maximum eye rotation angle in degrees

        void Start()
        {
            LoadGazeDataFromCSV();
            SetNextFixationTime(); // Set the fixation time for the first point
        }

        void Update()
        {
            if (gazeData.Count == 0) return;

            // Advance the timer
            timer += Time.deltaTime;

            // Check if we need to switch to the next gaze point
            if (timer >= currentFixationTime)
            {
                timer = 0f; // Reset the timer
                currentIndex++;

                if (currentIndex >= gazeData.Count)
                {
                    currentIndex = 0; // Loop back to the start
                }

                SetNextFixationTime();
            }

            // Update gaze (rotate the sphere) smoothly
            Vector2 gazePoint = new Vector2(gazeData[currentIndex].x, gazeData[currentIndex].y);
            UpdateGaze(gazePoint.x, gazePoint.y);
        }

        private void LoadGazeDataFromCSV()
        {
            //TextAsset csvFile = Resources.Load<TextAsset>("Data-16-12"); // CSV file in Resources folder
            //TextAsset csvFile = Resources.Load<TextAsset>("Data-18-12"); // CSV file in Resources folder
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
                if (values.Length >= 3)
                {
                    // Parse gaze_X, gaze_Y, and fixationTime
                    if (float.TryParse(values[0], out float gazeX) &&
                        float.TryParse(values[1], out float gazeY) &&
                        float.TryParse(values[2], out float fixationTime))
                    {
                        // Store gaze data as (X, Y, fixationTime)
                        gazeData.Add(new Vector3(gazeX, gazeY, fixationTime / 1000f)); // Convert ms to seconds
                    }
                }
            }

            Debug.Log($"Loaded {gazeData.Count} gaze points.");
        }

        private void SetNextFixationTime()
        {
            if (gazeData.Count > 0)
            {
                currentFixationTime = gazeData[currentIndex].z; // Set the fixation time in seconds
            }
        }

        private void UpdateGaze(float posX, float posY)
        {
            // Map gaze coordinates to rotation angles
            float rotationX = Mathf.Lerp(-maxRotationAngle, maxRotationAngle, posY / screenHeight);
            float rotationY = Mathf.Lerp(-maxRotationAngle, maxRotationAngle, posX / screenWidth);

            // Apply the rotation to the eyeball (smoothly)
            Quaternion targetRotation = Quaternion.Euler(rotationX, -rotationY, 0f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
