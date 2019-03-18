using Catalog.API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Catalog.API.Tests
{
    public class MessageTestData : IEnumerable<object[]>
    {

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { GetMessages() };
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private IEnumerable<object> GetMessages()
        {            
            for (int i = 0; i < 5; i++)
                yield return 
                    new Message {
                        Id = i + 1,
                        Name = $"Name {i + 1}",
                        Body = $"Boday {i + 1}"
                    };
        }
    }
}
