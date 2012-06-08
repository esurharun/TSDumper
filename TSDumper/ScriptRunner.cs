////////////////////////////////////////////////////////////////////////////////// 
//                                                                              //
//      Copyright (C) 2005-2012 nzsjb, Harun Esur                                           //
//                                                                              //
//  This Program is free software; you can redistribute it and/or modify        //
//  it under the terms of the GNU General Public License as published by        //
//  the Free Software Foundation; either version 2, or (at your option)         //
//  any later version.                                                          //
//                                                                              //
//  This Program is distributed in the hope that it will be useful,             //
//  but WITHOUT ANY WARRANTY; without even the implied warranty of              //
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                //
//  GNU General Public License for more details.                                //
//                                                                              //
//  You should have received a copy of the GNU General Public License           //
//  along with GNU Make; see the file COPYING.  If not, write to                //
//  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.       //
//  http://www.gnu.org/copyleft/gpl.html                                        //
//                                                                              //  
//////////////////////////////////////////////////////////////////////////////////


using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using DomainObjects;
using GeneralScriptInterface;
using Microsoft.CSharp;

namespace GeneralScriptInterface
{
    public interface IScriptType1
    {
       

        string before_start(string filepath, int tuner_idx);

        string after_finish(string filepath, int tuner_idx);
    }
}

namespace TSDumper

{
    internal static class ScriptRunner
    {
        private static IScriptType1 get_interface_from_file(string script_file_path)
        {
            var stream_reader = new StreamReader(script_file_path);
            string code = stream_reader.ReadToEnd();
            stream_reader.Close();

            Assembly compiled_script = CompileCode(code);

            if (compiled_script != null)
            {
                IScriptType1 script = get_interface(compiled_script);


                return script;
            }

            return null;
        }

        public static string run_after_finish(string filepath, int tuner_idx, string script_file_path)
        {
            IScriptType1 script = get_interface_from_file(script_file_path);

            if (script != null)
            {
                return script.after_finish(filepath, tuner_idx);
            }

            return null;
        }

        public static string run_before_start(string filepath, int tuner_idx, string script_file_path)
        {
            IScriptType1 script = get_interface_from_file(script_file_path);

            if (script != null)
            {
                return script.before_start(filepath, tuner_idx);
            }

            return null;
        }


        private static Assembly CompileCode(string code)
        {
            // Create a code provider
            // This class implements the 'CodeDomProvider' class as its base. All of the current .Net languages (at least Microsoft ones)
            // come with thier own implemtation, thus you can allow the user to use the language of thier choice (though i recommend that
            // you don't allow the use of c++, which is too volatile for scripting use - memory leaks anyone?)
            var csProvider = new CSharpCodeProvider();

            // Setup our options
            var options = new CompilerParameters();
            options.GenerateExecutable = false; // we want a Dll (or "Class Library" as its called in .Net)
            options.GenerateInMemory = true;
                // Saves us from deleting the Dll when we are done with it, though you could set this to false and save start-up time by next time by not having to re-compile
            // And set any others you want, there a quite a few, take some time to look through them all and decide which fit your application best!

            // Add any references you want the users to be able to access, be warned that giving them access to some classes can allow
            // harmful code to be written and executed. I recommend that you write your own Class library that is the only reference it allows
            // thus they can only do the things you want them to.
            // (though things like "System.Xml.dll" can be useful, just need to provide a way users can read a file to pass in to it)
            // Just to avoid bloatin this example to much, we will just add THIS program to its references, that way we don't need another
            // project to store the interfaces that both this class and the other uses. Just remember, this will expose ALL public classes to
            // the "script"
            options.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            // Compile our code
            CompilerResults result;
            result = csProvider.CompileAssemblyFromSource(options, code);

            if (result.Errors.HasErrors)
            {
                // TODO: report back to the user that the script has errored
                Logger.Instance.Write("An error occured while compiling script code");

                return null;
            }

            if (result.Errors.HasWarnings)
            {
                // TODO: tell the user about the warnings, might want to prompt them if they want to continue
                // runnning the "script"

                Logger.Instance.Write("Script has warnings in compilations");
            }

            return result.CompiledAssembly;
        }

        private static IScriptType1 get_interface(Assembly script)
        {
            // Now that we have a compiled script, lets run them
            foreach (Type type in script.GetExportedTypes())
            {
                foreach (Type iface in type.GetInterfaces())
                {
                    if (iface == typeof (IScriptType1))
                    {
                        // yay, we found a script interface, lets create it and run it!

                        // Get the constructor for the current type
                        // you can also specify what creation parameter types you want to pass to it,
                        // so you could possibly pass in data it might need, or a class that it can use to query the host application
                        ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                        if (constructor != null && constructor.IsPublic)
                        {
                            // lets be friendly and only do things legitimitely by only using valid constructors

                            // we specified that we wanted a constructor that doesn't take parameters, so don't pass parameters
                            var scriptObject =
                                constructor.Invoke(null) as IScriptType1;
                            if (scriptObject != null)
                            {
                                //Lets run our script and display its results
                                //MessageBox.Show(scriptObject.RunScript(50));
                                return scriptObject;
                            }
                            else
                            {
                                Logger.Instance.Write("Script object could not created");
                                // hmmm, for some reason it didn't create the object
                                // this shouldn't happen, as we have been doing checks all along, but we should
                                // inform the user something bad has happened, and possibly request them to send
                                // you the script so you can debug this problem
                            }
                        }
                        else
                        {
                            Logger.Instance.Write("Script object could not created");
                            // and even more friendly and explain that there was no valid constructor
                            // found and thats why this script object wasn't run
                        }
                    }
                }
            }

            return null;
        }
    }
}