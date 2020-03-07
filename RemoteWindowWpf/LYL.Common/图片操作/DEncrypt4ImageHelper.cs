using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Dispath
{
    //图片加密解密
    public class DEncrypt4ImageHelper
    {
        //public DEncrypt4ImageHelper() { }
        //#region 加密方法 图片加密
        ///// <summary>
        ///// 图片加密
        ///// </summary>
        ///// <param name="filePath">源文件</param>
        ///// <param name="savePath">保存为文件名称</param>
        ///// <param name="keyStr">密钥</param>
        //public static void EncryptFile(string filePath, string savePath, string keyStr)
        //{
        //    //通过des加密
        //    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //    //通过流打开文件
        //    FileStream fs = File.OpenRead(filePath);
        //    //获取文件二进制字符
        //    byte[] inputByteArray = new byte[fs.Length];
        //    //读流文件
        //    fs.Read(inputByteArray, 0, (int)fs.Length);
        //    //关闭流
        //    fs.Close();
        //    //获得加密字符串二进制字符
        //    byte[] keyByteArray = Encoding.Default.GetBytes(keyStr);
        //    //计算指定字节组指定区域哈希值
        //    SHA1 ha = new SHA1Managed();
        //    byte[] hb = ha.ComputeHash(keyByteArray);
        //    //加密密钥数组
        //    byte[] sKey = new byte[8];
        //    //加密变量
        //    byte[] sIV = new byte[8];
        //    for (int i = 0; i < 8; i++)
        //        sKey[i] = hb[i];
        //    for (int i = 8; i < 16; i++)
        //        sIV[i - 8] = hb[i];
        //    //获取加密密钥
        //    des.Key = sKey;
        //    //设置加密初始化向量
        //    des.IV = sIV;
        //    MemoryStream ms = new MemoryStream();
        //    CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        //    cs.Write(inputByteArray, 0, inputByteArray.Length);
        //    cs.FlushFinalBlock();
        //    fs = File.OpenWrite(savePath);
        //    foreach (byte b in ms.ToArray())
        //    {
        //        fs.WriteByte(b);
        //    }
        //    fs.Close();
        //    cs.Close();
        //    ms.Close();
        //}
        //#endregion
        //#region 解密方法 图片解密
        ///// <summary>
        ///// 图片解密
        ///// </summary>
        ///// <param name="filePath">源文件</param>
        ///// <param name="savePath">保存文件</param>
        ///// <param name="keyStr">密钥</param>
        //public static void DecryptFile(string filePath, string savePath, string keyStr)
        //{
        //    //通过des解密
        //    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //    //通过流读取文件
        //    FileStream fs = File.OpenRead(filePath);
        //    //获取文件二进制字符
        //    byte[] inputByteArray = new byte[fs.Length];
        //    //读取流文件
        //    fs.Read(inputByteArray, 0, (int)fs.Length);
        //    //关闭流
        //    fs.Close();
        //    //密钥数组
        //    byte[] keyByteArray = Encoding.Default.GetBytes(keyStr);
        //    //定义哈希变量
        //    SHA1 ha = new SHA1Managed();
        //    //计算指定字节组指定区域哈希值
        //    byte[] hb = ha.ComputeHash(keyByteArray);
        //    //加密密钥数组
        //    byte[] sKey = new byte[8];
        //    //加密变量
        //    byte[] sIV = new byte[8];
        //    for (int i = 0; i < 8; i++)
        //        sKey[i] = hb[i];
        //    for (int i = 8; i < 16; i++)
        //        sIV[i - 8] = hb[i];
        //    //获取加密密钥
        //    des.Key = sKey;
        //    //加密变量
        //    des.IV = sIV;
        //    MemoryStream ms = new MemoryStream();
        //    CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        //    cs.Write(inputByteArray, 0, inputByteArray.Length);
        //    cs.FlushFinalBlock();
        //    fs = File.OpenWrite(savePath);
        //    foreach (byte b in ms.ToArray())
        //    {
        //        fs.WriteByte(b);
        //    }
        //    fs.Close();
        //    cs.Close();
        //    ms.Close();
        //}



        //public static string GetMD5(string sDataIn)
        //{
        //    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        //    byte[] bytValue, bytHash;
        //    bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn);
        //    bytHash = md5.ComputeHash(bytValue);
        //    md5.Clear();
        //    string sTemp = "";
        //    for (int i = 0; i < bytHash.Length; i++)
        //    {
        //        sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
        //    }
        //    return sTemp.ToLower();
        //}
        //#endregion


        ///// <summary>
        ///// md5加密
        ///// </summary>
        ///// <param name="Text"></param>
        ///// <param name="sKey"></param>
        ///// <returns></returns>
        //public static string MD5Encrypt(string Text, string sKey)
        //{
        //    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //    byte[] inputByteArray = Encoding.Default.GetBytes(Text);
        //    des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
        //    des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
        //    System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //    CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
        //    cs.Write(inputByteArray, 0, inputByteArray.Length);
        //    cs.FlushFinalBlock();
        //    StringBuilder ret = new StringBuilder();
        //    foreach (byte b in ms.ToArray())
        //    {
        //        ret.AppendFormat("{0:X2}", b);
        //    }
        //    return ret.ToString();
        //}


        ///// <summary> 
        /////  MD5解密
        ///// </summary>  
        //public static string MD5Decrypt(string Text, string sKey)
        //{
        //    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //    int len;
        //    len = Text.Length / 2;
        //    byte[] inputByteArray = new byte[len];
        //    int x, i;
        //    for (x = 0; x < len; x++)
        //    {
        //        i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
        //        inputByteArray[x] = (byte)i;
        //    }
        //    des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
        //    des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
        //    System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //    CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        //    cs.Write(inputByteArray, 0, inputByteArray.Length);
        //    cs.FlushFinalBlock();
        //    return Encoding.Default.GetString(ms.ToArray());
        //}
    }
}
