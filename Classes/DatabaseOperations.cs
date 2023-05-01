using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace Secure_Facial_Recognition_Security_System_using_EmguCV
{
    /// <summary>
    /// Loads users' data from a SQL Server local database and registers new users
    /// </summary>
    internal class DatabaseOperations
    {
        #region Variables Declaration 
        private SqlConnection DBConnection;
        private static readonly string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Application.StartupPath + "\\Database\\Database1.mdf;Integrated Security=True;";
        // Holds SQL Command execution error result
        public static string DatabaseOperationStatus;
        private string UserNameValue = null;
        private byte[] UserPasscodeValue = null;
        private byte[] UserImageValue = null;
        private byte[] UserIDValue = null;
        #endregion

        /// <summary>
        /// Class Destructor
        /// </summary>
        #region Class Destructor
        ~DatabaseOperations()
        {
            DBConnection = null;
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// Validates input data and registers user data if validated 
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="UserPasscode"></param>
        /// <param name="UserImage"></param>
        /// <param name="UserID"></param>
        /// <returns>Boolean [False if there is an error]</returns>
        #region Data Validation and Registration
        public bool ValidateDataAndRegisterUser(string UserName, byte[] UserPasscode, byte[] UserImage, byte[] UserID)
        {
            // Holds returning value. False By default, sets to True if operations succeed
            bool ReturningValue = false;
            try

            {
                // Data Validation Rules
                if (string.IsNullOrWhiteSpace(UserName) || UserPasscode == null || UserImage == null || UserID == null)
                {
                    // Data validation failure
                    return ReturningValue;
                }

                // Data validated
                UserNameValue = UserName;
                UserPasscodeValue = UserPasscode;
                UserImageValue = UserImage;
                UserIDValue = UserID;
                // Calls Registering method
                RegisterNewUser();
                // Successful Operation
                ReturningValue = true;
            }
            // UnSuccessful Operation
            catch (Exception e)
            {
                // Saves error message for further analysis if needed
                DatabaseOperationStatus = e.Message;
                ReturningValue = false;
            }

            return ReturningValue;
        }
        #endregion

        /// <summary>
        /// Loads users' data into a data table
        /// </summary>
        /// <param name="UsersDataTable"></param>
        /// <remarks>out DataTable</remarks>
        /// <returns>Boolean [False if there is an error]</returns>
        #region Data Loading
        protected bool LoadUserData(out DataTable UsersDataTable)
        {
            // Holds returning value. False By default, sets to True if operations succeed
            bool ReturningValue = false;
            // Declares a new data table which holds data
            DataTable dts = new DataTable();
            try
            {
                // SQL query to load all records of the 'Users' table
                string query = "Select * from Users";
                // Instantiates a SqlConnection using predefined ConnectionString for command execution
                DBConnection = new SqlConnection(ConnectionString);
                DBConnection.Open();
                // Instantiates a SqlDataAdapter using SqlCommand to fill the data table with users' data
                SqlCommand cmd = new SqlCommand(query, DBConnection);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dts);
                // Closes SqlConnection
                Disconnect();
                // Successful Operation
                ReturningValue = true;

            }

            // UnSuccessful Operation
            catch (Exception e)
            {
                // Saves error message for further analysis if needed
                Disconnect();
                DatabaseOperationStatus = e.Message;
                ReturningValue = false;
            }

            // 
            UsersDataTable = dts;

            return ReturningValue;

        }
        #endregion

        /// <summary>
        /// Registers user's data into the database
        /// </summary>
        /// <returns></returns>
        #region Data Registration
        private bool RegisterNewUser()
        {
            // Holds returning value. False By default, sets to True if operations succeed
            bool ReturningValue = false;

            try
            {
                // SQL query to load all records of the 'Users' table
                String query = "INSERT INTO Users (UserName,UserPasscode,UserImage,UserID) VALUES (@UserNameValue,@UserPasscodeValue,@UserImageValue,@UserIDValue)";
                // Instantiates a SqlConnection using predefined ConnectionString for command execution
                DBConnection = new SqlConnection(ConnectionString);
                DBConnection.Open();
                // Instantiates a SqlCommand to be executed
                SqlCommand cmd = new SqlCommand(query, DBConnection);
                // Adds values to the SqlCommand
                cmd.Parameters.AddWithValue("@UserNameValue", UserNameValue);
                cmd.Parameters.AddWithValue("@UserPasscodeValue", UserPasscodeValue);
                cmd.Parameters.AddWithValue("@UserImageValue", UserImageValue);
                cmd.Parameters.AddWithValue("@UserIDValue", UserIDValue);
                // Holds SqlCommand execution result
                int result = cmd.ExecuteNonQuery();
                // Disconnects SqlConnection
                Disconnect();

                // Unsuccessful Operation
                if (result < 0)
                {
                    ReturningValue = false;
                }
                // Successful Operation
                else
                {
                    ReturningValue = true;
                }




            }
            // Unsuccessful Operation
            catch (Exception e)
            {
                // Saves error message for further analysis if needed
                Disconnect();
                DatabaseOperationStatus = e.Message;
                ReturningValue = false;
            }
            return ReturningValue;
        }
        #endregion

        /// <summary>
        /// Converts an image to byte[] array which is to be saved in the database
        /// </summary>
        /// <param name="img"></param>
        /// <returns>Byte[] array</returns>
        #region Image to Byte[] Conversion
        public static byte[] ImageToByte(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                stream.Close();
                byteArray = stream.ToArray();
            }
            return byteArray;
        }
        #endregion


        /// <summary>
        /// Disconnects SQLConnection
        /// </summary>
        #region SQL Connection Disconnection
        public void Disconnect()
        {
            DBConnection.Close();
        }
        #endregion
        #endregion




    }
}
