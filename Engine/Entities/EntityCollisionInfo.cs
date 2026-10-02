using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformsEngine.Engine.Entities
{
    public class EntityCollisionInfo
    {
        public HashSet<Entity> Targets { get; set; } = new HashSet<Entity>();
        public bool IsOnFloor { get; set; } = false;
        public bool IsOnLeftWall { get; set; } = false;
        public bool IsOnRightWall { get; set; } = false;
        public bool IsOnCeiling { get; set; } = false;

        public bool IsOnWall { get => IsOnLeftWall || IsOnRightWall; } 
        public bool IsColliding { get => IsOnWall || IsOnFloor || IsOnCeiling; }

        public void Clear() 
        {
            Targets.Clear();
            IsOnFloor = false;
            IsOnLeftWall = false;
            IsOnRightWall = false;
            IsOnCeiling = false;
        }
    }
}