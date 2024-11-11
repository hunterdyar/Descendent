using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proc
{
    public class BSPNode : IEnumerable<BSPNode>
    {
        public bool IsLeaf => _isLeaf;
        private bool _isLeaf = true;

        public BSPNode ChildA => childA;
        private BSPNode childA;
        public BSPNode ChildB => childB;
        private BSPNode childB;

        public Vector2Int Position => _position;
        /// <summary>
        /// Bottom Left
        /// </summary>
        private Vector2Int _position;
        public Vector2Int Size => _size;
        private Vector2Int _size;

        // private ProtoLevel protoLevel;
        public BSPNode(Vector2Int position, Vector2Int size)
        {
            _position = position;
            _size = size;
            _isLeaf = true;
        }
        
        public void Split()
        {
            _isLeaf = false;
            bool horizontal = Random.value < 0.5f;
            if (horizontal)
            {
                int split = 2;
                if (_size.x > 4)
                {
                    split = Random.Range(2, _size.x - 2);
                }
                var size = new Vector2Int(split, _size.y);
                childA = new BSPNode(_position , size);
           
                size = new Vector2Int(_size.x-split, _size.y);
                childB = new BSPNode(new Vector2Int(_position.x+split, _position.y), size);
            }   
            else
            {
                int split = 2;
                if (_size.y > 4)
                {
                    split = Random.Range(2, _size.y - 2);
                }
                
                var size = new Vector2Int(_size.x, split);
                childA = new BSPNode(_position , size);
           
                size = new Vector2Int(_size.x, _size.y-split);
                childB = new BSPNode(new Vector2Int(_position.x, _position.y+split), size);
            }
        }

        
        public static BSPNode Generate(int depth, int width, int height)
        {
            var root = new BSPNode(Vector2Int.zero, new Vector2Int(width, height));
            if (depth > 0)
            {
                RecursiveGenerate(root, 0, depth);
            }

            return root;
        }
        private static void RecursiveGenerate(BSPNode from, int currentDepth, int maxDepth)
        {
            from.Split();
            currentDepth++;
            if (currentDepth < maxDepth)
            {
                if (!from.childA._isLeaf)
                {
                    RecursiveGenerate(from.childA, currentDepth, maxDepth);
                }

                if (!from.childB._isLeaf)
                {
                    RecursiveGenerate(from.childB, currentDepth, maxDepth);
                }
            }
        }

        public IEnumerator<BSPNode> GetEnumerator()
        {
            if (_isLeaf)
            {
                yield return this;
            }
            else
            {
                if (childA._isLeaf)
                {
                    yield return childA;
                }
                else
                {
                    foreach (var grandchild in childA)
                    {
                        yield return grandchild;
                    }
                }
                if (childB._isLeaf)
                {
                    yield return childA;
                }
                else
                {
                    foreach (var grandchild in childB)
                    {
                        yield return grandchild;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}