﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using IniParser;

namespace SKInjector
{
    static class Extensions
    {
        #region DateTime
        public static bool ExpiredSince(this DateTime dateTime, int minutes)
        {
            return (dateTime - DateTime.Now).TotalMinutes < minutes;
        }
        #endregion
        #region DirectoryInfo
        public static DirectoryInfo Combine(this DirectoryInfo directory, params string[] paths)
        {
            return new DirectoryInfo(Path.Combine(directory.FullName, string.Join(Path.DirectorySeparatorChar.ToString(), paths)));
        }
        #endregion
        #region FileInfo
        public static string FileNameWithoutExtension(this FileInfo file) {
            return Path.GetFileNameWithoutExtension(file.Name);
        }
        /*public static string Extension(this FileInfo file) {
            return Path.GetExtension(file.Name);
        }*/
        public static void AppendLine(this FileInfo file, string line)
        {
            try {
                if (!file.Exists) file.Create();
                File.AppendAllLines(file.FullName, new string[] { line });
            } catch { }
        }
#endregion
        #region Object
        public static object ToJson(this object obj, bool indented = true) {
            return JsonConvert.SerializeObject(obj, (indented ? Formatting.Indented : Formatting.None), new JsonConverter[] { new StringEnumConverter() });
        }
#endregion
        #region String
        // public static string AppendLine(this string str, )
        public static string Remove(this string Source, string Replace)
        {
            return Source.Replace(Replace, string.Empty);
        }
        public static string ReplaceLastOccurrence(this string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);
            if(place == -1)
                return Source;
            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
        public static string Ext(this string text, string extension)
        {
            return text + "." + extension;
        }
        public static string Quote(this string text)
        {
            return SurroundWith(text, "\"");
        }
        public static string Enclose(this string text)
        {
            return SurroundWith(text, "(",")");
        }
        public static string SurroundWith(this string text, string surrounds)
        {
            return surrounds + text + surrounds;
        }
        public static string SurroundWith(this string text, string starts, string ends)
        {
            return starts + text + ends;
        }
#endregion
        #region List
        public static T PopAt<T>(this List<T> list, int index) {
            T r = list[index];
            list.RemoveAt(index);
            return r;
        }
        public static string Join<T>(this IEnumerable<T> list, string seperator) {
            return string.Join(seperator, list);
        }
        public static bool ContainsAny<TSource>(this IEnumerable<TSource> source, params TSource[] values) {
            return source.Any(x => values.Contains(x));
        }
        public static string Join(this ProcessModuleCollection modules, string seperator, bool useFileName = true)
        {
            var ret = "";
            foreach (ProcessModule module in modules)
            {
                ret += " " + (useFileName ? module.FileName : module.ModuleName);
            }
            return ret;
        }
#endregion
        #region Uri
        private static readonly Regex QueryRegex = new Regex(@"[?&](\w[\w.]*)=([^?&]+)");
        public static IReadOnlyDictionary<string, string> ParseQueryString(this Uri uri)
        {
            var match = QueryRegex.Match(uri.PathAndQuery);
            var paramaters = new Dictionary<string, string>();
            while (match.Success)
            {
                paramaters.Add(match.Groups[1].Value, match.Groups[2].Value);
                match = match.NextMatch();
            }
            return paramaters;
        }
#endregion
        #region Enum
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null) {
                FieldInfo field = type.GetField(name);
                if (field != null) {
                    DescriptionAttribute attr =  Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null) {
                        return attr.Description;
                    }
                }
            }
    return null;
}
#endregion
        #region Task
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout) {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource()) {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task) {
                    timeoutCancellationTokenSource.Cancel();
                    return await task;  // Very important in order to propagate exceptions
                } else {
                    return default(TResult);
                }
            }
        }
        #endregion
        #region Process
        public static bool Is64Bit(this Process process)
        {
            if (!Environment.Is64BitOperatingSystem)
                return false;
            bool isWow64;
            if (!IsWow64Process(process.Handle, out isWow64))
                throw new Win32Exception();
            return !isWow64;
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);
        #endregion
        #region Config
        public static bool hasTrueKey(this IniParser.Model.KeyDataCollection keyDatas, string key)
        {
            return keyDatas.ContainsKey(key) && keyDatas[key].ToLower() == "true";
        }
        #endregion
    }
}
