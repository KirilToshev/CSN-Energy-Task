using CSharpFunctionalExtensions;
using log4net;
using System;
using System.IO;

namespace CSN_Energy_Task
{
    public static class FileValidation
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(BookStore));
        public static Result<T> Validate<T>(string path, Func<Result<T>> executedFunction)
        {
            try
            {
                return executedFunction();
            }
            catch (FileNotFoundException e)
            {
                return Result.Fail<T>(string.Format("The filename {0} is invalid. Please provide a valid path to the file", path));
            }
            catch (DirectoryNotFoundException e)
            {
                return Result.Fail<T>(string.Format("The directory path provided {0} is invalid. Please provide a valid path to the file", path));
            }
            catch (FileLoadException e)
            {
                return Result.Fail<T>("An error occured while loading the file. Please provide a valid file");
            }
            catch (Exception e)
            {
                //This exception must be logged!
                _log.Error(e.Message, e);
                throw e;
            }
        }
    }
}
