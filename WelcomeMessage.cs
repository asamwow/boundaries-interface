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

   public class WelcomeMessage : UnityMessage {

      public int entropicId;

      public bool isAdmin;

      public WelcomeMessage(int entropicId, int chunkX, int chunkY, bool isAdmin)
          : base(chunkX, chunkY) {
         this.entropicId = entropicId;
         this.isAdmin = isAdmin;
         messageType = 0;
      }

      public WelcomeMessage(string hex) : base(hex) {
         entropicId = (int)Convert.ToInt32(hex.Substring(18, 8), 16);
         if (Convert.ToInt32(hex.Substring(26, 1), 2) == 0) {
            isAdmin = false;
         } else {
            isAdmin = true;
         }
      }

      public override void Write(Stream stream) {
         base.Write(stream);
         string digit = entropicId.ToString("X8");
         for (int i = 0; i < 8; i++) {
            stream.WriteByte((byte)digit[i]);
         }
         byte boolByte = 0;
         if (isAdmin) {
            boolByte = 1;
         }
         digit = boolByte.ToString("X2");
         stream.WriteByte((byte)digit[1]);
      }
   }
}
