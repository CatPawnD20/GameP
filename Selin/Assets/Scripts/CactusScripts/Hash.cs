
namespace Assets.Scripts
{
    class Hash
    {
        public static int hash_quinary(byte[] q, int k)
        {
            int sum = 0;
            const int len = 13;
            int i;

            for (i = 0; i < len; i++)
            {
                sum += DpTables.dp[q[i], len - i - 1, k];

                k -= q[i];

                if (k <= 0)
                {
                    break;
                }
            }

            return sum;
        }
    }
}