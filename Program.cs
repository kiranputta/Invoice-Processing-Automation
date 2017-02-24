
using Classes;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using IPA.UI;
using System.Text;
using System.Runtime.InteropServices;

namespace IPA.UI
{
  internal static class Program
  {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [DllImport("kernel32.dll")]
      private static extern long GetVolumeInformation(string PathName, StringBuilder VolumeNameBuffer, UInt32 VolumeNameSize, ref UInt32 VolumeSerialNumber, ref UInt32 MaximumComponentLength, ref UInt32 FileSystemFlags, StringBuilder FileSystemNameBuffer, UInt32 FileSystemNameSize);

    [STAThread]
    private static void Main()
    {
      if (PublicVariables.ConnectionType == "SQL")
      {
        if (File.Exists(PublicVariables.RequisiteFolder + "\\ConnectionDetailsSQL." + PublicVariables.ConnectionInformationFileType))
        {
          PublicVariables.ConnectionStringSQL = Program.DoReadFirstLine("ConnectionDetailsSQL." + PublicVariables.ConnectionInformationFileType);
          if (PublicVariables.ConnectionStringSQL.Length == 0)
          {
            int num1 = (int) MessageBox.Show("Connection details not present. Kindly contact the vendor", PublicVariables.ApplicationName);
          }
          try
          {
            using (SqlConnection sqlConnection = new SqlConnection(PublicVariables.ConnectionStringSQL))
              sqlConnection.Open();
          }
          catch (Exception ex)
          {
            Cls_ErrorLog.Writelog(ex.Message + " - " + ex.StackTrace);
            int num2 = (int) MessageBox.Show("Connection details given are not correct. Kindly contact the vendor", PublicVariables.ApplicationName);
          }
        }
        else
        {
          int num3 = (int) MessageBox.Show("Connection details not present. Kindly contact the vendor", PublicVariables.ApplicationName);
        }
      }
      else if (PublicVariables.ConnectionType == "MYSQL")
      {
        if (File.Exists(PublicVariables.RequisiteFolder + "\\ConnectionDetailsMYSQL." + PublicVariables.ConnectionInformationFileType))
        {
          PublicVariables.ConnectionStringMySQL = Program.DoReadFirstLine("ConnectionDetailsMYSQL." + PublicVariables.ConnectionInformationFileType);
          if (PublicVariables.ConnectionStringMySQL.Length == 0)
          {
            int num1 = (int) MessageBox.Show("Connection details not present. Kindly contact the vendor", PublicVariables.ApplicationName);
          }
          MySqlConnection mySqlConnection = (MySqlConnection) null;
          try
          {
            mySqlConnection = new MySqlConnection(PublicVariables.ConnectionStringMySQL);
            if (mySqlConnection.State == ConnectionState.Closed)
              mySqlConnection.Open();
          }
          catch (Exception ex)
          {
            Cls_ErrorLog.Writelog(ex.Message + " - " + ex.StackTrace);
            int num2 = (int) MessageBox.Show("Connection details given are not correct. Kindly contact the vendor", PublicVariables.ApplicationName);
          }
          finally
          {
            mySqlConnection.Close();
          }
        }
        else
        {
          int num4 = (int) MessageBox.Show("Connection details not present. Kindly contact the vendor", PublicVariables.ApplicationName);
        }
      }
      string @string = ConfigurationManager.AppSettings["FilePath"].ToString();
      PublicVariables.InputFilePath = Path.Combine(@string, Cls_DB_Con.ExecuteSelect("select code_desc from configuration where code='SharedInputFolder'"));
      PublicVariables.ArchiveFolder = Path.Combine(@string, Cls_DB_Con.ExecuteSelect("select code_desc from configuration where code='ArchiveFolder'"));
      PublicVariables.QueueFolder = Path.Combine(@string, Cls_DB_Con.ExecuteSelect("select code_desc from configuration where code='QueueFolder'"));
      PublicVariables.AppLogFolder = Path.Combine(@string, Cls_DB_Con.ExecuteSelect("select code_desc from configuration where code='ApplicationLogsFolder'"));
      PublicVariables.ResultFolder = Path.Combine(@string, Cls_DB_Con.ExecuteSelect("select code_desc from configuration where code='OutFilePath'"));
      PublicVariables.PdfToTiffFolder = Path.Combine(@string, Cls_DB_Con.ExecuteSelect("select code_desc from configuration where code='ConvertToTiffFolder'"));
      PublicVariables.OutFileFolder = Path.Combine(@string, Cls_DB_Con.ExecuteSelect("select code_desc from configuration where code='OutFilePath'"));
      PublicVariables.ImageTempPath = Path.Combine(@string, Cls_DB_Con.ExecuteSelect("select code_desc from configuration where code='ImageTemp'"));
      if (!Directory.Exists(PublicVariables.InputFilePath))
        Directory.CreateDirectory(PublicVariables.InputFilePath);
      if (!Directory.Exists(PublicVariables.ArchiveFolder))
        Directory.CreateDirectory(PublicVariables.ArchiveFolder);
      if (!Directory.Exists(PublicVariables.QueueFolder))
        Directory.CreateDirectory(PublicVariables.QueueFolder);
      if (!Directory.Exists(PublicVariables.AppLogFolder))
        Directory.CreateDirectory(PublicVariables.AppLogFolder);
      if (!Directory.Exists(PublicVariables.ResultFolder))
        Directory.CreateDirectory(PublicVariables.ResultFolder);
      if (!Directory.Exists(PublicVariables.PdfToTiffFolder))
        Directory.CreateDirectory(PublicVariables.PdfToTiffFolder);
      if (!Directory.Exists(PublicVariables.ImageTempPath))
        Directory.CreateDirectory(PublicVariables.ImageTempPath);
      if (!Directory.Exists(PublicVariables.OutFileFolder))
        Directory.CreateDirectory(PublicVariables.OutFileFolder);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      //if (DoCheckLicense())
      //{
      //    Application.Run((Form)new Login());

      //}
      //else
      //{
      //    MessageBox.Show("License Expired, Please Contact Your Service Provider", "IPA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

      //}
      Application.Run((Form)new Login());
      //Application.Run((Form)new Form_Main());
    }
    #region Expiry date Validation

    private static bool DoCheckLicense()
    {
        try
        {
            if (File.Exists(Application.StartupPath + "\\Key.dll"))
            {
                string strFirstLine = DoReadFirstLine();
                string[] rs = strFirstLine.Split("-".ToCharArray());

                if (strFirstLine == "")
                {
                    return false;
                }
                else if (rs.Length == 2 && rs[0] == "MSESELPAM")
                {
                    return DoWritedll(rs[1]);
                }
                else
                {
                    string[] strSplit = strFirstLine.Split('^');

                    if (strSplit.Length == 4 && strSplit[2].ToUpper() != "FULL")
                    {
                        DateTime startdt = DateTime.Parse(strSplit[1]);
                        DateTime enddt = DateTime.Parse(strSplit[2]);
                        DateTime cmpdt = DateTime.Parse(strSplit[3]);

                        string strHD = GetVolumeSerial(Application.StartupPath.Substring(0, 1));
                        if (strSplit[0] != strHD)
                            return false;
                        else if (DateTime.Now < startdt || DateTime.Now > enddt.AddDays(1))
                            return false;
                        else if (DateTime.Now < cmpdt)
                            return false;
                        else
                        {
                            DoUpdatedll(strHD, startdt, enddt);
                            return true;

                        }
                    }
                    else if (strSplit.Length == 4 && strSplit[2].ToUpper() == "FULL")
                    {
                        string strHD = GetVolumeSerial(Application.StartupPath.Substring(0, 1));

                        if (strSplit[0] != strHD)
                            return false;
                        else
                            return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private static bool DoWritedll(string strvalidity)
    {
        try
        {
            double validity = 0;
            string strWrite = string.Empty;

            double.TryParse(strvalidity, out validity);

            FileStream fs = new FileStream(Application.StartupPath + "\\Key.dll", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            if (strvalidity.ToUpper() != "FULL")
                strWrite = GetVolumeSerial(Application.StartupPath.Substring(0, 1)) + "^" + DateTime.Now.ToString() + "^" + DateTime.Now.AddDays(validity).ToString() + "^" + DateTime.Now.ToString();
            else
                strWrite = GetVolumeSerial(Application.StartupPath.Substring(0, 1)) + "^" + DateTime.Now.ToString() + "^FULL^" + DateTime.Now.ToString();

            sw.WriteLine(Cls_Crypto.Encrypt(strWrite, "MAPLOPTIS93874"));
            sw.Close();
            fs.Close();
            sw.Dispose();
            fs.Dispose();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private static bool DoUpdatedll(string HD, DateTime dtStart, DateTime dtEnd)
    {
        string strWrite = "";
        try
        {
            FileStream fs = new FileStream(Application.StartupPath + "\\Key.dll", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            int i = Convert.ToInt32(dtEnd.Subtract(DateTime.Now).TotalDays);
            if (i == 0)
            {
                string str = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString() + " 11:59:59 PM";
                DateTime dt = DateTime.Parse(DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString() + " 11:59:59 PM");
                strWrite = HD + "^" + dtStart.ToString() + "^" + dtEnd.ToString() + "^" + dt.ToString();
            }
            else
            {
                strWrite = HD + "^" + dtStart.ToString() + "^" + dtEnd.ToString() + "^" + DateTime.Now.ToString();
            }
            sw.WriteLine(Cls_Crypto.Encrypt(strWrite, "MAPLOPTIS93874"));
            sw.Close();
            fs.Close();
            sw.Dispose();
            fs.Dispose();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private static string DoReadFirstLine()
    {
        string strLine;
        try
        {
            FileStream fs = new FileStream(Application.StartupPath + "\\Key.dll", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            strLine = sr.ReadLine();
            sr.Close();
            fs.Close();
            sr.Dispose();
            fs.Dispose();
            strLine = Cls_Crypto.Decrypt(strLine, "MAPLOPTIS93874");
            return strLine;
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public static string GetVolumeSerial(string strDriveLetter)
    {
        uint serNum = 0;
        uint maxCompLen = 0;
        StringBuilder VolLabel = new StringBuilder(256); // Label
        UInt32 VolFlags = new UInt32();
        StringBuilder FSName = new StringBuilder(256); // File System Name
        strDriveLetter += ":\\"; // fix up the passed-in drive letter for the API call
        long Ret = GetVolumeInformation(strDriveLetter, VolLabel, (UInt32)VolLabel.Capacity, ref serNum, ref maxCompLen, ref VolFlags, FSName, (UInt32)FSName.Capacity);
        return serNum.ToString("X");
        //return Convert.ToString(serNum);
    }

    #endregion
    public static string DoReadFirstLine(string DLLFileName)
    {
      try
      {
        FileStream fileStream = new FileStream(PublicVariables.RequisiteFolder + "\\" + DLLFileName, FileMode.Open, FileAccess.Read);
        StreamReader streamReader = new StreamReader((Stream) fileStream);
        string str = streamReader.ReadLine();
        streamReader.Close();
        fileStream.Close();
        streamReader.Dispose();
        fileStream.Dispose();
        return str;
      }
      catch (Exception ex)
      {
        return "";
      }
    }
  }
}
