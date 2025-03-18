using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    class MathHelper
    {
        public static mat4 MultiplyMatrices(List<mat4> matrices)
        {
            mat4 res = new mat4(1);

            for (int i = 0; i < matrices.Count; i++)
            {
                res = Multiply(matrices[i],res);
            }
            return res;
        }
        static mat4 Multiply(mat4 a, mat4 b)
        {
            mat4 res = new mat4(0);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {

                    for (int k = 0; k < 4; k++)
                    {
                        res[i, j] += a[k, j] * b[i, k];
                    }
                }
            }

            return res;
        }
    }
}
