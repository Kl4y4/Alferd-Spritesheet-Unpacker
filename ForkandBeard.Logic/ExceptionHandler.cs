using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ForkandBeard.Logic
{
    public class ExceptionHandler
    {
        private static Forms.ExceptionForm exceptionForm = null;
        private static object exceptionFormLock = new object();

        public static void HandleException(Exception ex, string toEmailAddress)
        {
            //HandleException(ex, toEmailAddress, null);
            Console.WriteLine(ex.Message);
        }

        public static void HandleException(Exception ex, string toEmailAddress, IWin32Window owner)
        {
            lock (exceptionFormLock)
            {
                if (exceptionForm == null)
                {
                    exceptionForm = new Forms.ExceptionForm(ex, toEmailAddress);
                    if (owner != null)
                    {
                        try
                        {
                            exceptionForm.ShowDialog(owner);
                        }
                        catch
                        {
                            exceptionForm.ShowDialog();
                        }
                    }
                    else
                    {
                        exceptionForm.ShowDialog();
                    }
                }
                else
                {
                    exceptionForm.SetException(ex);
                    exceptionForm.SetEmailAddress(toEmailAddress);
                    if (exceptionForm.IsClosed)
                    {
                        exceptionForm.IsClosed = false;
                        if (owner != null)
                        {
                            exceptionForm.ShowDialog(owner);
                        }
                        else
                        {
                            exceptionForm.ShowDialog();
                        }
                    }
                }
            }
        }

        public static void HandleException(Exception ex)
        {
            //HandleException(ex, String.Empty);
            Console.WriteLine(ex.Message);
        }
    }
}
