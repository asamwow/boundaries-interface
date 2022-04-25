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

   public class CellMessage : UnityMessage {

      public Cell cell;

      public byte height;

      public CellMessage(Cell cell, int chunkX, int chunkY, int cellHeight) : base(chunkX, chunkY) {
         this.cell = cell;
         this.height = (byte)cellHeight;
         messageType = 1;
      }

      public CellMessage(string hex) : base(hex) {
         height = (byte)Convert.ToInt32(hex.Substring(18, 2), 16);
         cell = new Cell(hex.Substring(20));
      }

      public override void Write(Stream stream) {
         base.Write(stream);
         string digit = height.ToString("X2");
         for (int i = 0; i < 2; i++) {
            stream.WriteByte((byte)digit[i]);
         }
         cell.Write(stream);
      }
   }
}
