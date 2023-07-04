using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NullTest
{
    class Program
    {
        //裝函式
        delegate void ShowMethod(MemoryStream a, int b);
        static void Main(string[] args)
        {
            //將各自的bytes長度存入
            Queue<int> num = new Queue<int>();

            // 建立一個MemoryStream對象
            MemoryStream stream = new MemoryStream();
            DataSet(45.2, stream);
            // 寫入一些字符串
            string data = "Hello, World!";
            DataSet(data, stream);

            // 寫入一些數字
            int number = 999999999;
            DataSet(number, stream);


            // 寫入一個布林值
            bool flag = true;
            DataSet(flag, stream);

            //印出各自的bytes的原貌
            Show(stream);

        }
        static void DataSet<T>(T data, MemoryStream stream)
        {
            byte[] bytes;
            byte[] dataType = new byte[4];
            byte[] dataLength = new byte[4];

            if (data is int iData)
            {
                bytes = BitConverter.GetBytes(iData);
                dataType = BitConverter.GetBytes(1); ;
                StreamWriter(stream, dataType, dataLength, bytes);
            }
            //

            else if (data is bool bData)
            {
                bytes = BitConverter.GetBytes(bData);
                dataType=BitConverter.GetBytes(2); ;
                StreamWriter(stream, dataType, dataLength, bytes);
            }
            else
            {
                string sData = GetType(data.ToString());
                bytes = Encoding.UTF8.GetBytes(sData);
                dataType= BitConverter.GetBytes(0);
                StreamWriter(stream, dataType, dataLength, bytes);

            }
        }

        private static void StreamWriter(MemoryStream stream, byte[] dataType, byte[] dataLength, byte[] bytes)
        {
            dataLength[0] = Convert.ToByte(bytes.Length);
            stream.Write(dataType, 0, dataType.Length);
            stream.Write(dataLength, 0, dataLength.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        private static T GetType<T>(T data)
        {
            return data;
        }

        private static void Show(MemoryStream stream)
        {
            byte[] cmd = new byte[4];
            byte[] length = new byte[4];
            stream.Position = 0;
            //三種函式 存入delegate陣列內
            ShowMethod[] showMethods = { ShowString, ShowNumber, ShowBool };

            //依據stream.Position都基準往後讀取
            while (stream.Position < stream.Length)
            {
                stream.Read(cmd, 0, cmd.Length);
                stream.Read(length, 0, length.Length);

                ShowMethod choosenMethod = ChooseShowMethod(showMethods, DataType(cmd));
                int dataLength = BitConverter.ToInt32(length, 0);
                choosenMethod(stream, dataLength);

            }
        }


        private static string DataType(byte[] cmd)
        {
            int value = BitConverter.ToInt32(cmd, 0);
            if (value == 0) return "string";
            else if (value == 1) return "int";
            else if (value == 2) return "bool";
            else return "";
        }

        /// <summary>
        /// 依據index選出適當的函式
        /// </summary>
        /// <param name="showMethods">delegate陣列</param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static ShowMethod ChooseShowMethod(ShowMethod[] showMethods, string cmd)
        {

            if (cmd == "string") return (stream, count) => {
                byte[] stringBytes = new byte[count];
                stream.Read(stringBytes, 0, count);
                Console.WriteLine(Encoding.UTF8.GetString(stringBytes));
            };
            else if (cmd == "int") return ShowNumber;

            else if (cmd == "bool") return showMethods[2];

            else return null;

        }

        private static void ShowBool(MemoryStream stream, int count)
        {
            byte[] boolBytes = new byte[count];
            stream.Read(boolBytes, 0, count);
            Console.WriteLine(BitConverter.ToBoolean(boolBytes, 0));
        }
        private static void ShowNumber(MemoryStream stream, int count)
        {
            byte[] numberBytes = new byte[count];
            stream.Read(numberBytes, 0, count);
            Console.WriteLine(BitConverter.ToInt32(numberBytes, 0));
        }
        private static void ShowString(MemoryStream stream, int count)
        {
            byte[] stringBytes = new byte[count];
            stream.Read(stringBytes, 0, count);
            Console.WriteLine(Encoding.UTF8.GetString(stringBytes));

        }
    }
}
