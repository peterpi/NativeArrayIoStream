using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Collections;
using System.Linq;

public class MyTest
{
	private NativeArray<byte> m_native;
	private NativeArrayIoStream m_stream;

	[SetUp]
	public void Setup()
	{
		m_native = new NativeArray<byte>(1024, Allocator.Persistent);
		m_stream = new NativeArrayIoStream(m_native);
	}

	[TearDown]
	public void Teardown ()
	{
		m_stream.Dispose();
		if (m_native.IsCreated)
			m_native.Dispose();
	}


	[Test]
	public void DataIsWritten ()
	{
		byte[] managed = new byte[1024];
		new System.Random().NextBytes(managed);
		m_stream.Write(managed, 0, managed.Length);
		Assert.True(Enumerable.Range(0, m_native.Length).All(i => managed[i] == m_native[i]));
	}

	[Test]
	public void DataIsRead ()
	{
		byte[] managed = new byte[m_native.Length];
		new System.Random().NextBytes(managed);
		m_native.CopyFrom(managed);

		// Clear the managed array, ready to read the values back in.
		for (int i = 0; i < managed.Length; ++i)
			managed[i] = 0;

		m_stream.Read(managed, 0, managed.Length);
		Assert.True(Enumerable.Range(0, m_native.Length).All(i => managed[i] == m_native[i]));
	}


	[Test]
	public void WritePastEnd ()
	{
		byte[] managed = new byte[m_native.Length + 1];
		TestDelegate write = () => m_stream.Write(managed, 0, managed.Length);
		Assert.Throws(typeof(System.ArgumentException), write);
	}

	[Test]
	public void ReadPastEnd ()
	{
		byte[] managed = new byte[m_native.Length];
		// This should work:
		m_stream.Read(managed, 0, m_native.Length);
		// But reading another byte should fail.
		Assert.Throws(typeof(System.ArgumentOutOfRangeException), () => m_stream.ReadByte());
	}


	[Test]
	public void CanDisposeTwice ()
	{
		m_stream.Dispose();
		m_stream.Dispose(); // Doesn't throw exceptions etc.
	}


	[Test]
	public void DoesntDisposeArray()
	{
		m_stream.Dispose();
		Assert.True(m_native.IsCreated);
		m_native.Dispose();
		Assert.False(m_native.IsCreated);
	}


	[Test]
	public void UseAfterClose ()
	{
		m_stream.Close();
		m_stream.ReadByte();
	}


	[Test]
	public void CanSeekBeg()
	{
		for (int i=0; i<m_native.Length; ++i)
			m_stream.WriteByte((byte)(i % 256));

		// Assert that the bytes were actually written.
		Assert.True(Enumerable.Range(0, m_native.Length).All(i => m_native[i] == (byte)(i % 256)));

		// Now the actual test:  Assert that seeking works.
		m_stream.Seek(23, System.IO.SeekOrigin.Begin);
		Assert.True(m_stream.ReadByte() == 23);
	}
}



