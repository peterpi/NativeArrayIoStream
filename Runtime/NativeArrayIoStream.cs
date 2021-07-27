using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Collections;

public class NativeArrayIoStream : Stream
{
	private NativeArray<byte> m_array;

	private long m_pos;
	public override long Position { get => m_pos; set => m_pos = value; }


	private FileAccess m_access;

	[System.Flags]
	public enum Flags
	{
		NONE = 0
	}
	private Flags m_flags;

	public NativeArrayIoStream(NativeArray<byte> array, FileAccess access = FileAccess.ReadWrite, Flags flags = Flags.NONE)
	{
		m_array = array;
		m_access = access;
		m_flags = flags;
	}


	public override int Read(byte[] buffer, int offset, int count)
	{
		NativeArray<byte>.Copy(m_array, (int) m_pos, buffer, offset, count);
		m_pos += count;
		return count;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		NativeArray<byte>.Copy(buffer, offset, m_array, (int)m_pos, count);
		m_pos += count;
	}

	public override void Flush()
	{
	}

	public override long Length => m_array.Length;
	public override bool CanRead => (m_access & FileAccess.Read) != 0;
	public override bool CanWrite => (m_access & FileAccess.Write) != 0;


	public override bool CanSeek => true;

	public override bool CanTimeout => false;


	public override long Seek(long offset, SeekOrigin origin)
	{
		switch (origin)
		{
			case SeekOrigin.Begin:
				m_pos = offset;
				break;
			case SeekOrigin.Current:
				m_pos += offset;
				break;
			case SeekOrigin.End:
				m_pos = Length + offset;
				break;
		}
		return m_pos;
	}

	public override void SetLength(long value)
	{
		throw new System.NotSupportedException();
	}
}
