using System;
using System.Net.Http.Headers;

namespace ValuesAndReferences
{
    class Program
    {
        static void Main(string[] args)
        {
            MyStruct struct1 = new MyStruct();
            MyStruct struct2 = struct1;
            struct2.contents = 100;

            MyClass class1 = new MyClass();
            MyClass class2 = class1;
            class2.Contents = 100;

            Console.WriteLine("Value types: {0},{1}", struct1.contents, struct2.contents);
            Console.WriteLine("Reference types: {0},{1}", class1.Contents, class2.Contents);

        }
        struct MyStruct
        {
            public int contents;
        }
        class MyClass
        {
            public int Contents = 0;
        }
    }
}
