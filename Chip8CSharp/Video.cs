namespace Chip8CSharp
{
    public class Video
    {
        const uint SCREEN_WIDTH = 64;
        const uint SCREEN_HEIGHT = 32;
        const uint QUEUE_SIZE = 0x100;
        const uint QUEUE_OFFSET = 0;

        public uint[] videoBuffer = new uint[SCREEN_WIDTH * SCREEN_HEIGHT]; // video buffer
        public uint[] frameBuffer = new uint[SCREEN_WIDTH * SCREEN_HEIGHT]; // used as the real display

        uint offset_count = 0;
        uint offset_limit = QUEUE_OFFSET;
        bool copyFlag = false;
        ushort[] opcodeQueue = new ushort[QUEUE_SIZE];
        uint queuePointer = 0;

        void enqueue(ushort opcode)
        {
            opcodeQueue[queueMask((int)(queuePointer++))] = opcode;
        }

        void emptyqueue()
        {
            queuePointer = 0;
        }

        void findOpcodefromQueue(ushort opcode)
        {
            for (int i = 0; i < queuePointer; i++)
            {
                if ((opcode == opcodeQueue[queueMask(i)]) && (++offset_count > offset_limit))
                {
                    copyFlag = true;
                    offset_count = 0;
                    return;
                }
            }
        }

        int queueMask(int i)
        {
            if (i > QUEUE_SIZE - 1)
            {
                copyFlag = true;
            }
            return (int)(i % QUEUE_SIZE);
        }

        public void copyToFbuffer()
        {
            for (int i = 0; i < (SCREEN_WIDTH * SCREEN_HEIGHT); i++)
                frameBuffer[i] = videoBuffer[i];
        }

        public void clearVBuffer()
        {
            for (int i = 0; i < (SCREEN_WIDTH * SCREEN_HEIGHT); i++)
                videoBuffer[i] = 0;
            copyToFbuffer();
        }

        public void optimizations(ushort opcode)
        {
            findOpcodefromQueue(opcode);
            if (copyFlag)
            {
                emptyqueue();
                copyToFbuffer();
                copyFlag = false;
            }
            enqueue(opcode);
        }

        public void init(uint queue_offset)
        {
            this.offset_limit = queue_offset;

            clearVBuffer();
        }
    }
}
