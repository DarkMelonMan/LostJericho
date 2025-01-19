using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LabsonCS
{
    public class Area
    {
        public Transform SideTransform {get; private set;}
        public Vector2 SideArea { get; private set;}
        public LayerMask AreaLayer {  get; private set;}
        float sideAttackTransformX;
        public Area(Transform sideTransform, Vector2 sideArea, LayerMask areaLayer) {
            SideTransform = sideTransform;
            SideArea = sideArea;
            AreaLayer = areaLayer;
            sideAttackTransformX = SideTransform.localPosition.x;
        }
        public void ChangeDirection(int direction)
        {
            SideTransform.localPosition = new Vector2(direction * sideAttackTransformX, SideTransform.localPosition.y);
        }
    }
}
