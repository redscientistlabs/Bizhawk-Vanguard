﻿using System;
using System.Windows.Forms;
using SlimDX.Direct3D9;
using SlimDX.DirectSound;

namespace BizHawk.MultiClient
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try { Global.DSound = new DirectSound(); }
            catch {
                MessageBox.Show("Couldn't initialize DirectSound!");
                return;
            }

            try { Global.Direct3D = new Direct3D(); }
            catch {
                //can fallback to GDI rendering
            }

            try {
                Application.Run(new MainForm(args));
            } catch (Exception e) {
                MessageBox.Show(e.ToString(), "Oh, no, a terrible thing happened!");
            } finally {
                if (Global.DSound != null && Global.DSound.Disposed == false)
                    Global.DSound.Dispose();
                if (Global.Direct3D != null && Global.Direct3D.Disposed == false)
                    Global.Direct3D.Dispose();
            }
        }
    }
}
