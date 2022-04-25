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

   public class CommandMessage : UnityMessage {

      public int action;

      public CommandMessage(int action, int chunkX, int chunkY) : base(chunkX, chunkY) {
         this.action = action;
         messageType = 8;
      }

      public CommandMessage(string hex) : base(hex) {
         action = (int)Convert.ToInt32(hex.Substring(18, 8), 16);
      }

      public override void Write(Stream stream) {
         base.Write(stream);
         string digit = action.ToString("X8");
         for (int i = 0; i < 8; i++) {
            stream.WriteByte((byte)digit[i]);
         }
      }
   }
}
