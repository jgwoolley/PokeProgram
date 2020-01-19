using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileHelpers
{
    public class FileHelper
    {
        public static List<string> GetPath(FileInfo file)
        {
            List<string> path = new List<string>();
            path.Add(file.Name);
            return GetPath(file.Directory, path);

        }

        public static List<string> GetPath(DirectoryInfo dir)
        {
            return GetPath(dir, new List<string>());

        }

        private static List<string> GetPath(DirectoryInfo directory, List<string> path)
        {
            DirectoryInfo parent = directory;
            while (parent != null)
            {
                path.Add(directory.Name);
                parent = directory.Parent;
            }
            return path;
        }

        public static DirectoryInfo CreateDirectory(DirectoryInfo parent, params string[] paths)
        {
            return new DirectoryInfo(Append(parent.FullName, paths));
        }

        public static DirectoryInfo CreateDirectory(params string[] paths)
        {
            return new DirectoryInfo(Append(paths));
        }

        public static DirectoryInfo CreateDirectory(DirectoryInfo parent, List<string> paths)
        {
            return new DirectoryInfo(Append(parent.FullName, paths));
        }

        public static DirectoryInfo CreateDirectory(List<string> paths)
        {
            return new DirectoryInfo(Append(paths));
        }

        public static FileInfo CreateFile(DirectoryInfo parent, params string[] paths)
        {
            return new FileInfo(Append(parent.FullName, paths));
        }

        public static FileInfo CreateFile(params string[] paths)
        {
            return new FileInfo(Append(paths));
        }

        public static FileInfo CreateFile(DirectoryInfo parent, List<string> paths)
        {
            return new FileInfo(Append(parent.FullName, paths));
        }

        public static FileInfo CreateFile(List<string> paths)
        {
            return new FileInfo(Append(paths));
        }

        public static string Append(string parent, params string[] paths)
        {
            StringBuilder buff = new StringBuilder();
            buff.Append(parent);
            foreach (string path in paths)
            {
                buff.Append(Path.DirectorySeparatorChar + path);
            }

            return buff.ToString();
        }

        public static string Append(params string[] paths)
        {
            StringBuilder buff = new StringBuilder();
            foreach (string path in paths)
            {
                buff.Append(Path.DirectorySeparatorChar + path);
            }

            return buff.ToString();
        }

        public static string Append(string parent, List<string> paths)
        {
            StringBuilder buff = new StringBuilder();
            buff.Append(parent);
            foreach (string path in paths)
            {
                buff.Append(Path.DirectorySeparatorChar + path);
            }

            return buff.ToString();
        }

        public static string Append(List<string> paths)
        {
            StringBuilder buff = new StringBuilder();
            foreach (string path in paths)
            {
                buff.Append(Path.DirectorySeparatorChar + path);
            }

            return buff.ToString();
        }

        public static void Write<T,I>(FileHelperWrapper<T,I> fileHelper) where T : FileHelperWrapper<T, I> where I : FileSystemInfo
        {
            Console.Write(fileHelper.FullName);
        }

        public static void WriteLine<T, I>(FileHelperWrapper<T, I> fileHelper) where T : FileHelperWrapper<T, I> where I : FileSystemInfo
        {
            Console.WriteLine(fileHelper.FullName);
        }
    }

    public class DirectoryWrapper : FileHelperWrapper<DirectoryWrapper, DirectoryInfo>
    {
        public DirectoryWrapper(DirectoryInfo file) : base(FileHelper.GetPath(file))
        {

        }

        public DirectoryWrapper(params string[] path) : base(path)
        {

        }

        public DirectoryWrapper(List<string> path) : base(path)
        {

        }

        public DirectoryWrapper(DirectoryWrapper fileHelperWrapper) : base(fileHelperWrapper)
        {

        }

        public DirectoryWrapper() : base()
        {

        }

        public override DirectoryInfo CreateInfo(List<string> Path)
        {
            return FileHelper.CreateDirectory(Path);
        }

        public override DirectoryWrapper Create(List<string> Path)
        {
            return new DirectoryWrapper(Path);
        }

        public static DirectoryWrapper Create(params string[] path) {
            return new DirectoryWrapper(path);
        }

        public static FileWrapper Create(FileInfo file)
        {
            return new FileWrapper(file);
        }

        public static DirectoryWrapper Create()
        {
            return new DirectoryWrapper();
        }

        public override DirectoryWrapper Copy()
        {
            return Create(this.Path);
        }

        public FileWrapper GetChildFile(List<string> path) {
            List<string> list = new List<string>(this.Path);
            foreach (string p in Path)
            {
                list.Add(p);
            }
            return new FileWrapper(list);
        }

        public FileWrapper GetChildFile(params string[] path)
        {
            List<string> list = new List<string>(this.Path);
            foreach (string p in Path)
            {
                list.Add(p);
            }
            return new FileWrapper(list);
        }

        public DirectoryWrapper GetChildDirectory(List<string> path)
        {
            List<string> list = new List<string>(this.Path);
            foreach (string p in Path)
            {
                list.Add(p);
            }
            return new DirectoryWrapper(list);
        }

        public DirectoryWrapper GetChildDirectory(params string[] path)
        {
            List<string> list = new List<string>(this.Path);
            foreach (string p in Path)
            {
                list.Add(p);
            }
            return new DirectoryWrapper(list);
        }
    }

    public class FileWrapper: FileHelperWrapper<FileWrapper, FileInfo>
    {
        public FileWrapper(FileInfo file) : base(FileHelper.GetPath(file))
        {

        }


        public FileWrapper(params string[] path) : base(path)
        {

        }

        public FileWrapper(List<string> path) : base(path)
        {

        }

        public FileWrapper(FileWrapper fileHelperWrapper) : base(fileHelperWrapper)
        {

        }

        public FileWrapper() : base()
        {

        }

        public override FileWrapper Create(List<string> Path)
        {
            return new FileWrapper(Path);
        }

        public override FileInfo CreateInfo(List<string> Path)
        {
            return FileHelper.CreateFile(Path);
        }

        public static FileWrapper Create(params string[] path)
        {
            return new FileWrapper(path);
        }

        public static FileWrapper Create(FileInfo file)
        {
            return new FileWrapper(file);
        }

        //public static FileWrapper Create(List<string> path)
        //{
        //    return new FileWrapper(path);
        //}

        public static FileWrapper Create()
        {
            return new FileWrapper();
        }

        public override FileWrapper Copy()
        {
            return Create(this.Path);
        }
    }

    public abstract class FileHelperWrapper<T,I> where T: FileHelperWrapper<T,I> where I: FileSystemInfo
    {
        public string FullName
        {
            get
            {
                return CreateInfo().FullName;
            }
        }

        public string Name
        {
            get
            {
                return Path[^1];
            }
        }

        public List<string> Path { get; }

        public FileHelperWrapper(params string[] path)
        {
            this.Path = new List<string>(path);
        }

        public FileHelperWrapper(List<string> path)
        {
            this.Path = new List<string>(path);
        }

        public FileHelperWrapper(T fileHelperWrapper)
        {
            this.Path = new List<string>(fileHelperWrapper.Path);
        }

        public FileHelperWrapper()
        {
            this.Path = new List<string>();
            Path.Add(AppDomain.CurrentDomain.BaseDirectory);
        }

        public void Add(params string[] paths)
        {
            foreach (string path in paths)
            {
                this.Path.Add(path);
            }
        }

        public DirectoryWrapper GetParent()
        {
            DirectoryWrapper directory = new DirectoryWrapper(this.Path);
            directory.Remove();
            return directory;
        }

        public DirectoryWrapper GetParent(int count)
        {

            List<string> newPath = new List<string>(this.Path);

            if (newPath.Count < count)
            {
                throw new IndexOutOfRangeException("this.Path.Count = " + this.Path.Count + " given " + count + ".");
            }

            for (int i = 0; i < count; i++)
            {
                newPath.RemoveAt(newPath.Count - 1);
            }
            return new DirectoryWrapper(newPath);
        }

        public void Remove()
        {
            Path.RemoveAt(Path.Count - 1);
        }

        public abstract I CreateInfo(List<string> Path);

        public I CreateInfo()
        {
            return CreateInfo(Path);
        }

        public abstract T Create(List<string> Path);

        public T CreateHelperWrapper()
        {
            return Create(Path);
        }

        public override string ToString()
        {
            return FileHelper.Append(Path);

        }

        public static explicit operator I(FileHelperWrapper<T,I> wrapper)
        {
            return wrapper.CreateInfo();
        }

        public abstract T Copy();

    }
}
