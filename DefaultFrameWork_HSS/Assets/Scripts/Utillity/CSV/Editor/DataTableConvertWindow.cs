using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using ExcelDataReader;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace HSS
{
    public class DataTableConvertWindow : EditorWindow
    {
        // ----- Param -----
        private readonly string readPath_All = "/DataTable/Excel";
        private readonly string savePath_DB = "/Resources/CSV/";
        private readonly string SEARCH_DIRECTORY_PATH = "search_directory_path";

        private string searchDirectory = "";
        private string savedSearchDirectory = "";
        private string projectDataTableDirectory = "Assets/DataTable/Excel";

        private static StringBuilder sbSuccess = new StringBuilder();
        private static StringBuilder sbFail = new StringBuilder();
        private static Regex regexComma = new Regex(",");

        private StringBuilder sbProgress = new StringBuilder();

        // ----- Init -----

        [MenuItem("ExcelData/Convert All DataTable")]
        private static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<DataTableConvertWindow>();
            window.titleContent = new GUIContent("Convert All DataTable");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(" ");

            GUILayout.Label(" [ Search Folder Path ] ");

            DrawSelectDirectoryUI(ref searchDirectory, "Search Path");

            if (string.IsNullOrEmpty(searchDirectory) == false)
            {
                if (savedSearchDirectory.Equals(searchDirectory) == false)
                {
                    savedSearchDirectory = searchDirectory;
                    //PlayerPrefs.SetString("search_directory_path", savedSearchDirectory);
                }

                if (GUILayout.Button("Move All DataTable Files & Create CSV file", GUILayout.Height(40))) 
                    CopyAllDataFiles();
            }

            GUILayout.Label(sbProgress.ToString());
            GUILayout.FlexibleSpace();
        }

        // ----- Set ----- 

        // ----- Get ----- 

        private string GetDataPath_Application(string path) => $"{Application.dataPath}{path}";

        /// <summary>
        /// 파일명 가져오기
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="strSplit"></param>
        /// <param name="strReplace"></param>
        /// <returns></returns>
        private string GetFileName(string filePath, char strSplit)
        {
            return filePath.Split(strSplit)[0];
        }

        /// <summary>
        /// 파일명 가져오기
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="strSplit"></param>
        /// <param name="strReplace"></param>
        /// <returns></returns>
        private string GetFileName(string filePath, char strSplit, string strReplace)
        {
            string name = string.Empty;
            string[] nameArray = filePath.Split(strSplit);
            return nameArray.Length > 0 ? nameArray[nameArray.Length - 1].Replace(strReplace, string.Empty) : name;
        }

        // ----- Main ----- 

        // 엑셀 데이터 -> CSV 변환
        public void ConvertProcess_DataTable_All()
        {
            sbSuccess.Clear();
            sbFail.Clear();
            sbSuccess.AppendLine("# File List #");

            DirectoryInfo di = new DirectoryInfo(GetDataPath_Application(readPath_All));

            // 루트 폴더안에 있는 엑셀 파일
            foreach (System.IO.FileInfo file in di.GetFiles("*.xls*"))
            {
                ConvertToCSV(GetDataPath_Application($"{readPath_All}/{file.Name}"), savePath_DB);
                HSSLog.Log("File.Name : " + file.Name);
            }

            // 루트 폴더내 다른 폴더에 있는 엑셀 파일
            foreach (DirectoryInfo folder in di.GetDirectories())
            {
                HSSLog.Log("Folder.Name : " + folder.Name);
                foreach (System.IO.FileInfo file in folder.GetFiles("*.xls*"))
                {
                    ConvertToCSV(GetDataPath_Application($"{readPath_All}/{folder.Name}/{file.Name}"), savePath_DB);
                    HSSLog.Log("File.Name : " + file.Name);
                }
            }

            EditorUtility.DisplayDialog("Data Convert & Save Success.", sbSuccess.ToString(), "OK");
            if (sbFail.Length != 0)
                EditorUtility.DisplayDialog("Data Convert Fail", sbFail.ToString(), "OK");

            AssetDatabase.Refresh();
        }

        private void CopyAllDataFiles()
        {
            sbProgress.Clear();

            if (string.IsNullOrEmpty(projectDataTableDirectory)) 
                return;

            DirectoryInfo dicInfo = new DirectoryInfo(searchDirectory);
            List<FileInfo> files = new List<FileInfo>(dicInfo.GetFiles());

            Dictionary<string, string> dicFileFolder = new Dictionary<string, string>
            {
                { "ExampleFile" , "ExampleFolder"}
            };

            foreach (FileInfo info in files)
            {
                if (info.Name.Contains("DS_Store"))
                    continue;

                string name = GetFileName(info.Name, '.');

                if (!dicFileFolder.TryGetValue(name, out string folderName))
                    continue;

                string destFolderPath = $"{projectDataTableDirectory}/{folderName}";
                string destFilePath = $"{destFolderPath}/{info.Name}";

                if (!Directory.Exists(destFolderPath))
                    AssetDatabase.CreateFolder(projectDataTableDirectory, folderName);

                DeleteFile($"{destFilePath}.xlsx");
                DeleteFile($"{destFilePath}.meta");

                File.Copy(info.FullName, destFilePath, true);
                sbProgress.AppendLine(info.Name);
            }

            ConvertProcess_DataTable_All();
            AssetDatabase.Refresh();
        }

        // ----- Logic ----- 

        // startRow 기본 3 -> 기획자의 임의 표기, 이름, 자료형
        private void ConvertToCSV(string filePath, string savePath, int startRow = 3)
        {
            HSSLog.Log($" From => {filePath} \n To => {savePath}");

            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    ConvertProcess(reader.AsDataSet(), GetDataPath_Application(savePath), GetFileName(filePath, '/', ".xlsx"), startRow);
                }
            }
            catch (Exception ex)
            {
                sbFail.AppendLine($"{filePath} 변환 실패: {ex.Message}");
            }
        }

        private void ConvertProcess(DataSet result, string savePath, string fileName, int startRow)
        {
            try
            {
                int columns = result.Tables[0].Columns.Count;
                int rows = result.Tables[0].Rows.Count;
                StringBuilder sb = new StringBuilder();

                for (int x = startRow; x < rows; x++)
                {
                    for (int y = 0; y < columns; y++)
                    { 
                        // column
                        string str = result.Tables[0].Rows[x][y].ToString();

                        //// 비어있는 행 건너뛸 수 있음
                        //string str = result.Tables[0].Rows[x][y]?.ToString().Trim();
                        //if (string.IsNullOrEmpty(str))
                        //    continue;

                        // 쉼표 오류 방지 - csv 파일은 쉼표로 구분된 텍스트 파일이기 때문
                        if (str.Contains(','))
                            str = regexComma.Replace(str, "u002c");

                        if (str.StartsWith('{') && str.EndsWith('}'))
                            str = $"\"{str}\"";  //string.Format("\"{0}\"", str);

                        sb.Append(str);
                        if (y < columns - 1)
                            sb.Append(",");
                    }
                    sb.AppendLine();
                }

                string filePath = $"{savePath}{fileName}.csv";
                DeleteFile(filePath);                           // Delete CSV

                sbSuccess.AppendLine(fileName);
                FileSave(fileName, sb.ToString(), savePath);    // Save CSV

                DeleteFile($"{filePath}.meta");                 // meta 파일 제거. (리로드용)
            }
            catch (System.Exception e)
            {
                sbSuccess.Append(e.Message + "\n");
            }
        }

        /// <summary>
        /// 파일 저장
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="filePath"></param>
        public void FileSave(string name, string value, string filePath)
        {
            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                File.WriteAllText($"{filePath}{name}.csv", value, Encoding.UTF8);

                //string fileName = $"{name}.csv";
                //Stream fileStream = new FileStream(filePath + fileName, FileMode.OpenOrCreate, FileAccess.Write);
                //StreamWriter outStream = new StreamWriter(fileStream, new UTF8Encoding(true));
                //outStream.WriteLine(value);
                //outStream.Close();
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog($"{name}.csv File Save Fail", e.Message, "OK");
            }
        }

        /// <summary>
        /// 파일 삭제
        /// - csv / meta 파일 제거시 사용한다.
        /// </summary>
        /// <param name="path"></param>
        private void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public void DrawSelectDirectoryUI(ref string path, string title)
        {
            EditorGUILayout.BeginHorizontal();

            path = EditorGUILayout.TextField(title, path);
            // 찾은 주소를 저장해서 불러오게 하려면 아래의 주석 해제
            //if (GUILayout.Button("Get Search Path", GUILayout.Width(160)))
            //{
            //    string savedPath = PlayerPrefs.GetString("search_directory_path", "");
            //    path = savedPath;
            //}

            if (GUILayout.Button("Folder Search", GUILayout.Width(100)))
            {
                var selected = EditorUtility.OpenFolderPanel(title, path, "default");
                if (string.IsNullOrEmpty(selected) != true)
                    path = selected;
            }

            if (GUILayout.Button("Folder Open", GUILayout.Width(100)) && !string.IsNullOrEmpty(path))
                System.Diagnostics.Process.Start(path);

            EditorGUILayout.EndHorizontal();
        }
    }
}