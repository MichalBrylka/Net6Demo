namespace Net6Demo
{
    internal class UnsafeExamples
    {
        public static void Test()
        {
            //DEMO show AI coding 
            unsafe
            {
                int* number = (int*)NativeMemory.Alloc(sizeof(int));
                *number = 42;

                int* numbers = (int*)NativeMemory.Alloc(2, sizeof(int));
                numbers[0] = 42;
                numbers[1] = 420;

                numbers++;
                var n = *numbers;


                NativeMemory.Free(number);
                NativeMemory.Free(numbers);
            }
        }
    }
}
