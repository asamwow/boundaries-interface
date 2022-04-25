/* Boundaries - Interfaces and Models
 * Original Version Written by Samuel Jahnke 2022
 *
 * This library is free software: you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Library General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.  If not, see
 * <https://www.gnu.org/licenses/>.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace boundaries.Models {

   public class Matter {
      // names currently DO NOT match up since they have changed so much
      // its planned to make matter configurable in realtime through gui
      private enum MatterType {
         Iron,    // 0
         Ice,     // 1
         Soft,    // 2
         Hard,    // 3
         Fibrous, // 4
         Bone,    // 5
         Gold,    // 6
         Oxygen,  // 7
         Gas,     // 8
         Silt,    // 9
         Sand,    // 10
         Clay,    // 11
         Pebbles, // 12
         ARock,   // 13
         Stone,   // 14
         Vacuum   // 15
      }

      private MatterType matterType;

      public bool isSolid;

      public bool isWater;

      public bool isBeach;

      public bool isShore;

      public bool isRoad;

      public bool isLand;

      public bool isForest;

      public Matter(byte matterValue) {
         matterType = (MatterType)matterValue;
         isSolid = true;
         if (matterType == MatterType.Vacuum) {
            isSolid = false;
         }
         isWater = false;
         if (matterType == MatterType.Iron || matterType == MatterType.Hard ||
             matterType == MatterType.Stone) {
            isWater = true;
         }
         isBeach = false;
         if (matterType == MatterType.Pebbles || matterType == MatterType.Silt) {
            isBeach = true;
         }
         isShore = false;
         if (matterType == MatterType.Soft) {
            isShore = true;
         }
         isRoad = false;
         if (matterType == MatterType.Gold) {
            isRoad = true;
         }
         isLand = false;
         if (matterType == MatterType.Clay) {
            isLand = true;
         }
         isForest = false;
         if (matterType == MatterType.Ice || matterType == MatterType.Oxygen) {
            isForest = true;
         }
      }

      public Matter(int matterValue) : this((byte)matterValue) {}

      public override bool Equals(object obj) {
         var item = obj as Matter;

         if (item == null) {
            return false;
         }

         return this.matterType.Equals(item.matterType);
      }

      public bool Equals(byte matterValue) { return GetMatterTypeValue() == matterValue; }

      public byte GetMatterTypeValue() { return (byte)matterType; }

      public override int GetHashCode() { return ((byte)this.matterType).GetHashCode(); }

      public override string ToString() { return matterType.ToString(); }

      public static implicit operator byte(Matter matter) => (byte)matter.matterType;
      public static explicit operator Matter(byte matter) => new Matter(matter);

      public static implicit operator int(Matter matter) => (int)matter.matterType;
      public static explicit operator Matter(int matter) => new Matter(matter);

      public static int Count {
         get { return 16; }
      }

      public static Matter Hard {
         get { return new Matter(3); }
      }
      public static Matter Soft {
         get { return new Matter(2); }
      }
      public static Matter Fibrous {
         get { return new Matter(4); }
      }
      public static Matter Iron {
         get { return new Matter(0); }
      }
      public static Matter Stone {
         get { return new Matter(14); }
      }
      public static Matter Ice {
         get { return new Matter(1); }
      }
      public static Matter Bone {
         get { return new Matter(5); }
      }
      public static Matter Oxygen {
         get { return new Matter(7); }
      }
      public static Matter Gas {
         get { return new Matter(8); }
      }
      public static Matter Silt {
         get { return new Matter(9); }
      }
      public static Matter Sand {
         get { return new Matter(10); }
      }
      public static Matter Clay {
         get { return new Matter(11); }
      }
      public static Matter Pebbles {
         get { return new Matter(12); }
      }
      public static Matter ARock {
         get { return new Matter(13); }
      }
      public static Matter Gold {
         get { return new Matter(6); }
      }
      public static Matter Vacuum {
         get { return new Matter(15); }
      }
   }
}
