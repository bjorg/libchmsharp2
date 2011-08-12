﻿/*
 * libchmsharp2 - a C# port of chmlib
 * Copyright (C) 2011 MindTouch, Inc.
 * www.mindtouch.com  oss@mindtouch.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CHMsharp;

namespace test
{
    class Program
    {
        static string _outdir;

        static void Main(string[] args)
        {
            if (args.Length != 2) {
                Console.WriteLine("USAGE: test.exe <input file> <output directory>");
                return;
            }

            _outdir = args[1];
            Object o = new Object();

            chmFile chmf = chmFile.Open(args[0]);
            chmf.Enumerate(
                EnumerateLevel.Normal,
                new chmEnumerator(EnumeratorCallback),
                o);
            chmf.Close();
        }

        static EnumerateStatus EnumeratorCallback(chmFile file, chmUnitInfo ui, Object context)
        {
            if (!ui.path.EndsWith("/"))
                Console.WriteLine(file.FileName + ": " + ui.path);

            if (ui.length > 0) {
                byte[] buf = new byte[ui.length];
                long ret = file.RetrieveObject(ui, ref buf, 0, buf.Length);

                if (ret > 0) {
                    try {
                        FileInfo fi =
                            new FileInfo(Path.Combine(_outdir, ui.path.Trim('/')));
                            
                        List<DirectoryInfo> created;

                        fi.Directory.CreateDirectory(out created);
                        File.WriteAllBytes(fi.FullName, buf);
                    } catch (ArgumentException ex) {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return EnumerateStatus.Continue;
        }
    }
}
