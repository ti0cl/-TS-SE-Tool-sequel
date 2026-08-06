/*
   Copyright 2016-2022 LIPtoH <liptoh.codebase@gmail.com>

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
<<<<<<< HEAD
=======
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
>>>>>>> afbd459757509edaca43c6cee8e7b865a5e812ac
using TS_SE_Tool.Utilities;

namespace TS_SE_Tool
{
    public partial class FormMain
    {
        public unsafe string[] NewDecodeFile(string _savefile_path)
        {
            return NewDecodeFile(_savefile_path, true);
        }

        public unsafe string[] NewDecodeFile(string _savefile_path, bool _verbose)
        {
            if (_verbose)
                UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Info, "message_loading_save_file");
            if (_verbose)
                IO_Utilities.LogWriter("Loading file into memory: " + _savefile_path);

            var returnData = GetSaveFileFormat(_savefile_path);

            sbyte saveFileFormat = returnData.saveFileFormat;
            byte[] fileDataInBytes = returnData.fileDataInBytes;
            UInt32 buff = (UInt32)fileDataInBytes.Length;

            switch (saveFileFormat)
            {
                case 1:
                    // "SIIDEC_RESULT_FORMAT_PLAINTEXT";
                    {
                        FileDecoded = true;
                        string BigS = Encoding.UTF8.GetString(fileDataInBytes);
                        return BigS.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    }
                case 2:
                    // "SIIDEC_RESULT_FORMAT_ENCRYPTED";
                    {
                        if (_verbose)
                            UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Info, "message_decoding_save_file");
                        if (_verbose)
                            IO_Utilities.LogWriter("Decoding file: " + _savefile_path);

                        int result = -1;
                        uint newbuff = 0;
                        uint* newbuffP = &newbuff;

                        fixed (byte* ptr = fileDataInBytes)
                        {
                            result = SIIDecryptAndDecodeMemory(ptr, buff, null, newbuffP);
                        }

                        if (result == 0)
                        {
                            byte[] newFileData = new byte[(int)newbuff];

                            fixed (byte* ptr = fileDataInBytes)
                            {
                                fixed (byte* ptr2 = newFileData)
                                    result = SIIDecryptAndDecodeMemory(ptr, buff, ptr2, newbuffP);
                            }
                            if (_verbose)
                                UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Clear);

                            FileDecoded = true;
                            string BigS = Encoding.UTF8.GetString(newFileData);
                            return BigS.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                        }

                        return null;
                    }
                case 3:
                    // "SIIDEC_RESULT_FORMAT_BINARY";
                case 4:
                    // "SIIDEC_RESULT_FORMAT_3NK";
                    {
                        if (_verbose)
                            UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Info, "message_decoding_save_file");
                        if (_verbose)
                            IO_Utilities.LogWriter("Decoding file: " + _savefile_path);

                        int result = -1;
                        uint newbuff = 0;
                        uint* newbuffP = &newbuff;

                        fixed (byte* ptr = fileDataInBytes)
                        {
                            result = SIIDecodeMemory(ptr, buff, null, newbuffP);
                        }

                        if (result == 0)
                        {
                            byte[] newFileData = new byte[(int)newbuff];

                            fixed (byte* ptr = fileDataInBytes)
                            {
                                fixed (byte* ptr2 = newFileData)
                                    result = SIIDecodeMemory(ptr, buff, ptr2, newbuffP);
                            }
                            if (_verbose)
                                UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Clear);

                            FileDecoded = true;
                            string BigS = Encoding.UTF8.GetString(newFileData);
                            return BigS.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        }
                        return null;
                    }
                case -1:
                    // "SIIDEC_RESULT_GENERIC_ERROR";
                case 10:
                    // "SIIDEC_RESULT_FORMAT_UNKNOWN";
                case 11:
                    // "SIIDEC_RESULT_TOO_FEW_DATA";
                default:
                    // "UNEXPECTED_ERROR";
                    return null;
            }
        }

        private unsafe (sbyte saveFileFormat, byte[] fileDataInBytes) GetSaveFileFormat(string _savefile_path)
        {
            if (!File.Exists(_savefile_path))
            {
                IO_Utilities.LogWriter("Could not find file in: " + _savefile_path);
                UpdateStatusBarMessage.ShowStatusMessage(SMStatus.Error, "error_could_not_find_file");

                FileDecoded = false;
                return (-1, null);
            }    

            byte[] fileDataInBytes = File.ReadAllBytes(_savefile_path);

            sbyte saveFileFormat = -1;
            UInt32 buff = (UInt32)fileDataInBytes.Length;

            fixed (byte* ptr = fileDataInBytes)
            {
                saveFileFormat = (sbyte)SIIGetMemoryFormat(ptr, buff);
            }

            return (saveFileFormat, fileDataInBytes);
        }

<<<<<<< HEAD
=======
        private byte[] EncodeScsC(byte[] plainData, byte[] originalFileData)
        {
            const int headerSize = 56;
            byte[] key =
            {
                0x2a, 0x5f, 0xcb, 0x17, 0x91, 0xd2, 0x2f, 0xb6,
                0x02, 0x45, 0xb3, 0xd8, 0x36, 0x9e, 0xd0, 0xb2,
                0xc2, 0x73, 0x71, 0x56, 0x3f, 0xbf, 0x1f, 0x3c,
                0x9e, 0xdf, 0x6b, 0x11, 0x82, 0x5a, 0x5d, 0x0a
            };

            if (originalFileData.Length < headerSize || originalFileData[0] != (byte)'S' || originalFileData[1] != (byte)'c' || originalFileData[2] != (byte)'s' || originalFileData[3] != (byte)'C')
                throw new InvalidDataException("The original save does not have a valid ScsC header.");

            byte[] compressedData;
            using (MemoryStream compressedStream = new MemoryStream())
            {
                using (DeflaterOutputStream deflaterStream = new DeflaterOutputStream(compressedStream, new Deflater(6, false)))
                {
                    deflaterStream.Write(plainData, 0, plainData.Length);
                    deflaterStream.Finish();
                }

                compressedData = compressedStream.ToArray();
            }

            int paddedLength = (compressedData.Length + 15) / 16 * 16;
            byte[] paddedData = new byte[paddedLength];
            Buffer.BlockCopy(compressedData, 0, paddedData, 0, compressedData.Length);

            byte[] encryptedData;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                byte[] initializationVector = new byte[16];
                Buffer.BlockCopy(originalFileData, 36, initializationVector, 0, initializationVector.Length);
                aes.IV = initializationVector;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    encryptedData = encryptor.TransformFinalBlock(paddedData, 0, paddedData.Length);
            }

            byte[] result = new byte[headerSize + encryptedData.Length];
            Buffer.BlockCopy(originalFileData, 0, result, 0, headerSize);
            Buffer.BlockCopy(encryptedData, 0, result, headerSize, encryptedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes((uint)plainData.Length), 0, result, 52, sizeof(uint));

            return result;
        }

>>>>>>> afbd459757509edaca43c6cee8e7b865a5e812ac
        //SII decrypt
        [DllImport(@"libs/SII_Decrypt.dll", EntryPoint = "GetFileFormat")]
        public static extern Int32 SIIGetFileFormat(string FilePath);

        //unsafe
        [DllImport(@"libs/SII_Decrypt.dll", EntryPoint = "GetMemoryFormat")]
        public static extern unsafe Int32 SIIGetMemoryFormat(byte* InputMS, uint InputMSSize);

        [DllImport(@"libs/SII_Decrypt.dll", EntryPoint = "DecryptAndDecodeMemory")]
        public static extern unsafe Int32 SIIDecryptAndDecodeMemory(byte* InputMS, uint InputMSSize, byte* OutputMS, uint* OutputMSSize);

        [DllImport(@"libs/SII_Decrypt.dll", EntryPoint = "DecodeMemory")]
        public static extern unsafe Int32 SIIDecodeMemory(byte* InputMS, uint InputMSSize, byte* OutputMS, uint* OutputMSSize);

        private string SIIresultDecode (int inputR)
        {
            switch (inputR)
            {
                case -1:
                    return "SIIDEC_RESULT_GENERIC_ERROR";
                case 0:
                    return "SIIDEC_RESULT_SUCCESS";
                case 1:
                    return "SIIDEC_RESULT_FORMAT_PLAINTEXT";
                case 2:
                    return "SIIDEC_RESULT_FORMAT_ENCRYPTED";
                case 3:
                    return "SIIDEC_RESULT_FORMAT_BINARY";
                case 4:
                    return "SIIDEC_RESULT_FORMAT_3NK";
                case 10:
                    return "SIIDEC_RESULT_FORMAT_UNKNOWN";
                case 11:
                    return "SIIDEC_RESULT_TOO_FEW_DATA";
                case 12:
                    return "SIIDEC_RESULT_BUFFER_TOO_SMALL";
                default:
                    return "UNEXPECTED_ERROR";
            }
        }
    }
}