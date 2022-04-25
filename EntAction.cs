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

   public class EntAction {

      public const byte CellSize = Cell.CellSize;

      public enum Type {
         None,                   // 0
         MoveLeft,               // 1
         MoveRight,              // 2
         MoveChunkUp,            // 3
         MoveChunkDown,          // 4
         Jump,                   // 5
         Fall,                   // 6
         JumpLeft,               // 7
         JumpRight,              // 8
         JumpChunkUp,            // 9
         JumpChunkDown,          // 10
         FallLeft,               // 11
         FallRight,              // 12
         FallChunkUp,            // 13
         FallChunkDown,          // 14
         ForceFallDownChunk,     // 15
         ForceFallRight,         // 16
         ForceFallLeft,          // 17
         ForceFallFailure,       // 18
         ForceFallJumpChunkDown, // 19
         LeftClick,              // 20
         Update,                 // 21
         Create                  // 22
      }

      public Type type { get; private set; }

      public EntAction(byte actionValue) { type = (Type)actionValue; }

      public EntAction(Type type) { this.type = type; }

      public byte GetTypeValue() { return (byte)type; }

      public bool Equals(byte actionValue) { return GetTypeValue() == actionValue; }

      public override bool Equals(object obj) {
         var item = obj as EntAction;

         if (item == null) {
            return false;
         }

         return this.type.Equals(item.type);
      }
   }
}
