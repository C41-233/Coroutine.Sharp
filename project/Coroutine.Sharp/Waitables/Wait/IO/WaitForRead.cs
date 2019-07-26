using System;
using System.IO;

namespace Coroutines
{
    internal sealed class WaitForRead : WaitableTask<int>
    {

        public WaitForRead(Stream stream, byte[] buffer, int offset, int count)
        {
            stream.BeginRead(buffer, offset, count, ReadCallback, stream);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var stream = (Stream)ar.AsyncState;
            try
            {
                var nread = stream.EndRead(ar);
                Success(nread);
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }


    }

}
