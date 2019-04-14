using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WebMVC.Models;

namespace WebMVC.UnitTests
{
    public class MessagesTestData : IEnumerable<object[]>
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
                        Body = $"Body {i + 1}",
                        Category = string.Format("Cat{0}", (i % 2 == 0)? "1": "2")
                    };
        }
    }
}
