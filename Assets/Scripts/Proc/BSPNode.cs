using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Proc
{
    public class BSPNode : IEnumerable<BSPNode>
    {
        public bool IsLeaf => _isLeaf;
        private bool _isLeaf = true;

        public bool SplitHorizontal => _splitHorizontal;
        private bool _splitHorizontal;
        public BSPNode ChildA => childA;
        private BSPNode childA;
        public BSPNode ChildB => childB;
        private BSPNode childB;
        public BSPNode Parent => _parent;
        private BSPNode _parent;

        public Vector2Int Position => _position;
        /// <summary>
        /// Bottom Left
        /// </summary>
        private Vector2Int _position;
        public Vector2Int Size => _size;
        private Vector2Int _size;

        public Vector2Int ConnectionPoint => _connectionPoint;
        private Vector2Int _connectionPoint;

        public List<Vector2Int> InternalConnectionPoints => _internalConnectionPoints;
        private readonly List<Vector2Int> _internalConnectionPoints = new List<Vector2Int>();
        // private ProtoLevel protoLevel;
        public BSPNode(Vector2Int position, Vector2Int size, BSPNode parent = null)
        {
            _position = position;
            _size = size;
            _isLeaf = true;
            _parent = parent;
        }
        
        public bool Split(float maxRatio = 2)
        {
            if (_size.x <= 4 || _size.y <= 4)
            {
                return false;
            }
            
            _isLeaf = false;
            //force a split alkong the long-side if it's greater than the ratio (not too thin in either dimension)
            //todo: I might have this backwards? It's honestly hard to tell because it doesn't enforce the child dimensions, it inforces one layer up from any leaf node only.
            //e.g. It my still split into a thin node.
            if (_size.x*maxRatio < _size.y)
            {
                _splitHorizontal = false;
            }
            else if (_size.y*maxRatio < _size.x)
            {
                _splitHorizontal = true;
            }else{
                _splitHorizontal = Random.value < 0.5f;
            }
            if (_splitHorizontal)
            {
                int split = 3;
                if (_size.x > 5)
                {
                    split = Random.Range(3, _size.x - 3);
                }
                //todo: we can't split on the line that is the parents connection point, which should be in it's _internal...
                
                var connection = new Vector2Int(_position.x + split-1, _position.y+Random.Range(0, _size.y));
                _internalConnectionPoints.Add(connection);
                
                var size = new Vector2Int(split-1, _size.y);
                childA = new BSPNode(_position, size, this);
                childA._connectionPoint = connection - Vector2Int.right;
                
                size = new Vector2Int(_size.x-split, _size.y);
                childB = new BSPNode(new Vector2Int(_position.x+split, _position.y), size, this);
                childB._connectionPoint = connection + Vector2Int.right;

            }   
            else
            {
                int split = 3;
                if (_size.y > 5)
                {
                    split = Random.Range(3, _size.y - 3);
                }

                var connection = new Vector2Int(_position.x+ Random.Range(0, _size.x), _position.y + split -1);
                _internalConnectionPoints.Add(connection);
                
                var size = new Vector2Int(_size.x, split-1);
                childA = new BSPNode(_position , size, this);
                childA._connectionPoint = connection + Vector2Int.down;
                
                size = new Vector2Int(_size.x, _size.y-split);
                childB = new BSPNode(new Vector2Int(_position.x, _position.y+split), size, this);
                childB._connectionPoint = connection + Vector2Int.up;
            }

            return true;
        }

        
        public static BSPNode Generate(int maxDepth, int width, int height)
        {
            var root = new BSPNode(Vector2Int.zero, new Vector2Int(width, height));
            if (maxDepth > 0)
            {
                RecursiveGenerate(root, 0, maxDepth);
            }

            return root;
        }
        private static void RecursiveGenerate(BSPNode from, int currentDepth, int maxDepth)
        {
            from.Split();
            currentDepth++;
            if (currentDepth < maxDepth)
            {
                if (from._isLeaf)
                {
                    //split failed.
                    return;
                }

                RecursiveGenerate(from.childA, currentDepth, maxDepth);
                RecursiveGenerate(from.childB, currentDepth, maxDepth);
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

        public void DrawGizmos(Matrix4x4 gridToWorld)
        {
            Gizmos.color = _isLeaf ? Color.yellow : Color.magenta;
                var points = new [] {
                    _position, _position + new Vector2Int(0, _size.y) , _position + new Vector2Int(_size.x, _size.y), _position+new Vector2Int(_size.x,0)
                };
                float s = _isLeaf ? 0.99f : 1f;
                Gizmos.DrawLineStrip(points.Select(x=>gridToWorld.MultiplyPoint(new Vector3(x.x*s,x.y*s,0))).ToArray(),true);
            
            if (!_isLeaf)
            {
                ChildA.DrawGizmos(gridToWorld);
                ChildB.DrawGizmos(gridToWorld);
            }
        }
    }
}