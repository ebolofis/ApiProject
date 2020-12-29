using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Models.Models.Helper
{
    public class Node
    {
        /// <summary>
        /// '+' or '.'
        /// </summary>
        public char Operator { get; set; }

        /// <summary>
        /// List of Node's operants. 
        /// ex into expression A+B the operants are A,B. 
        /// </summary>
        public List<string> Operants { get; set; } = new List<string>();

        /// <summary>
        /// List of children nodes
        /// </summary>
        public List<Node> Children { get; set; } = new List<Node>();

        /// <summary>
        /// Node's level into the Tree structure
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// Parent Node
        /// </summary>
        public Node Parent { get; set; }
    }
}
