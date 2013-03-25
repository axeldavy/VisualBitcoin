namespace Data
{
	public class Block : WebRole.Models.Block
	{
		public Block(string hash, string version, string previousBlock,
			string merkleRoot, int time, int bits, int numberOnce, int size, int index,
			bool isInMainChain, int height, int receivedTime, string relayedBy, int numberOfTransactions)
			: base(hash, version, previousBlock, merkleRoot, time, bits, numberOnce,
			size, index, isInMainChain, height, receivedTime, relayedBy)
		{
		}
	}
}
