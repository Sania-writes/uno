#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.Graphics.Holographic
{
	#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial struct HolographicFrameId 
	{
		// Forced skipping of method Windows.Graphics.Holographic.HolographicFrameId.HolographicFrameId()
		#if __ANDROID__ || __IOS__ || NET461 || __WASM__ || __MACOS__
		public  ulong Value;
		#endif
	}
}