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
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace boundaries.Models {

   public class UnityMessage {

      public byte messageType;

      public int chunkX;
      public int chunkY;

      public UnityMessage(int chunkX, int chunkY) {
         this.chunkX = chunkX;
         this.chunkY = chunkY;
         messageType = 0;
      }

      public UnityMessage(string hex) {
         messageType = (byte)Convert.ToInt32(hex.Substring(0, 2), 16);
         chunkX = (int)Convert.ToInt32(hex.Substring(2, 8), 16);
         chunkY = (int)Convert.ToInt32(hex.Substring(10, 8), 16);
      }

      public virtual void Write(Stream stream) {
         string digit = messageType.ToString("X2");
         for (int i = 0; i < 2; i++) {
            stream.WriteByte((byte)digit[i]);
         }
         digit = chunkX.ToString("X8");
         for (int i = 0; i < 8; i++) {
            stream.WriteByte((byte)digit[i]);
         }
         digit = chunkY.ToString("X8");
         for (int i = 0; i < 8; i++) {
            stream.WriteByte((byte)digit[i]);
         }
      }
   }
}
