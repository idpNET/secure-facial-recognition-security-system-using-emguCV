using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using System.Drawing;
using System.Data;
using PBKDF2_hashing;
using System.Linq;
using Secure_Facial_Recognition_Security_System_using_EmguCV;
/// <summary>
/// EMGU.CV.LBPHFaceRecognizer trainer and Passcode validity checker
/// </summary>
class FaceDetectionOperations : DatabaseOperations, IDisposable
{

    #region Variables Declaration

    // Instantiates a new LBPHFaceRecognizer
    // Threshold is set to 90 by default which might work just fine in many situations
    // With higher threshold value, an unregistered face will be more likely to be recognized as registered face
    // With lower threshold value, a registered face will be more likely to be recognized as unregistered face [needs more training images] 
    FaceRecognizer Recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 90);
    // Assigns an Emgu.Cv.Image list variable to store training images used in users' face recognition
    List<Image<Gray, byte>> TrainingImages = new List<Image<Gray, byte>>();

    // Assigns Users' Names, Passcode, Images and IDs list variable to save and retrain users' data 
    List<string> Names_List = new List<string>();
    List<byte[]> Passcodes_List = new List<byte[]>();
    List<byte[]> SavedImages_List = new List<byte[]>();
    List<byte[]> UserIDs_List = new List<byte[]>();
    List<int> RecordsIndex_List = new List<int>();


    // Assigns EMGU.CV.LBPHFaceRecognizer parameters
    float LBPHDistance = 0;
    string LBPHLabel;


    //Assigns a boolean to check if user's data loaded
    public bool IsDataLoaded = false;


    #endregion

    #region Class constructor and methods
    /// <summary>
    /// Class Default Constructor (loads users' data)
    /// </summary>
    public FaceDetectionOperations()
    {
        if (IsDataLoaded)
        {
            Dispose();
        }
        // Loads Users' data if when class gets Initialized
        IsDataLoaded = LoadTrainingData();
    }

    /// <summary>
    /// Disposes loaded data and retrains the recognizer
    /// </summary>
    /// <returns>True, if data loading is successful</returns>
    public bool Retrain()
    {
        Dispose();
        return IsDataLoaded = LoadTrainingData();
    }

    /// <summary>
    ///  Recognizer training status
    /// </summary>
    /// <returns>True, if recognizer is successfully trained</returns>
    public bool DataLoaded
    {
        get { return IsDataLoaded; }
    }


    /// <summary>
    /// Recognizes a gray scale image using a trained LBPHFaceRecognizer
    /// </summary>
    /// <param name="Input_image"></param>
    /// <param name="LBPHThresh"></param>
    /// <returns>Null, if there is no faces matching the input image</returns>
    /// <returns>User's Name, if there is a face matching the input image</returns>
    /// 
    public string Recognise(Image<Gray, byte> Input_image, int LBPHThresh = -1)
    {
        // checks if data is already loaded successfully
        if (IsDataLoaded)
        {
            // Instantiates a FaceRecognizer.PredictionResult to detect any match to the input image
            FaceRecognizer.PredictionResult PredictionResults = Recognizer.Predict(Input_image);

            // There is no match for the input image
            if (PredictionResults.Label == -1)
            {
                LBPHLabel = null;
                LBPHDistance = 0;
                return LBPHLabel;
            }
            else
            {
                // Sets LBPHLabel and LBPHDistance to the corresponding values returned from FaceRecognizer.PredictionResult 
                LBPHLabel = Names_List[PredictionResults.Label];
                LBPHDistance = (float)PredictionResults.Distance;
                // Returns User's Name
                return LBPHLabel;

            }

        }
        // Returns a null value if data is not loaded successfully
        else return null;
    }




    /// <summary>
    /// Returns a float confidence value for potential false classification
    /// </summary>
    public float Get_LBPHDistance
    {
        get
        {
            return LBPHDistance;
        }
    }


    /// <summary>
    /// Class dispose method which clears all the data lists and calls the Garbage Collector
    /// </summary>
    public void Dispose()
    {
        RecordsIndex_List.Clear();
        Passcodes_List.Clear();
        UserIDs_List.Clear();
        Names_List.Clear();
        TrainingImages.Clear();
        GC.Collect();
    }


    /// <summary>
    /// Checks input passcode
    /// </summary>
    /// <param name="UserName"></param>
    /// <param name="UserPasscode"></param>
    /// <returns>True, if entered passcode is correct</returns>
    public bool CheckPasscode(string UserName, string UserPasscode)

    {
        // Iterates through database records using UserName column
        foreach (var item in Names_List)
        {

            // Checks if the input UserName matches any name in the Names_List items
            if (item.Equals(UserName))
            {
                // Assigns int value to hold index value of matching user, in the list
                int indx = Names_List.IndexOf(item);
                // Assigns byte[] array to hold matching user's salt value
                byte[] salt = UserIDs_List[indx];
                // Assigns byte[] array to hold matching user's passcode
                byte[] OriginalPassCodeHashValue = Passcodes_List[indx];
                // Computes matching user's passcode hash value using the input passcode and the loaded salt value
                byte[] PassCodeHashValue = HashComputation.ComputeBytesHash(UserPasscode, salt);


                // Checks if hash values are equal 
                if (PassCodeHashValue.SequenceEqual(OriginalPassCodeHashValue))
                {
                    // Entered passcode is correct
                    return true;
                }
                else
                {
                    // Entered passcode is incorrect
                    return false;
                }
            }
        }



        return false;

    }

    /// <summary>
    /// Loads data used for training EMGU.CV.LBPHFaceRecognizer
    /// </summary>
    /// <returns></returns>
    private bool LoadTrainingData()
    {

        // Holds returning value. False By default, sets to True if operations succeed
        bool ReturningValue = false;

        // Loads users' data and put them all into a data table using out modifier
        LoadUserData(out DataTable UsersDataTable);



        int IDIndex = 0;

        // Iterates through data table records and populating data lists
        foreach (DataRow row in UsersDataTable.Rows)
        {
            try
            {
                // Adds each user's encrypted picture into the SavedImages_List
                SavedImages_List.Add((byte[])row["UserImage"]);
                // Decrypts and converts each user's picture to be used in recognizer training 
                var arrayBinary = AESCryptography.Decrypt((byte[])row["UserImage"]);
                Image userPicture;
                using (MemoryStream ms = new MemoryStream(arrayBinary))
                {
                    userPicture = Image.FromStream(ms);
                }
                Bitmap masterImage = (Bitmap)userPicture;
                // Adds each user's decrypted picture into the TrainingImages list
                TrainingImages.Add(new Image<Gray, Byte>(masterImage));

                // Populates lists with data table records
                RecordsIndex_List.Add(IDIndex);
                Passcodes_List.Add((byte[])row["UserPasscode"]);
                UserIDs_List.Add((byte[])row["UserID"]);
                Names_List.Add(row["UserName"].ToString());
                IDIndex++;
            }

            // UnSuccessful operation
            catch (Exception)
            {
                Dispose();
                return false;
            }
        }

        // Checks if there is any image in the TrainingImages array
        if (TrainingImages.ToArray().Length != 0)
        {
            // Trains the EMGU.CV.LBPHFaceRecognizer with images in TrainingImages array
            Recognizer.Train(TrainingImages.ToArray(), RecordsIndex_List.ToArray());
            ReturningValue = true;
        }

        return ReturningValue;
    }

    #endregion
}

