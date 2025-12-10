using System;
using System.Runtime.InteropServices;

namespace DataChannelDotnet.Bindings
{
    public enum rtcState
    {
        RTC_NEW = 0,
        RTC_CONNECTING = 1,
        RTC_CONNECTED = 2,
        RTC_DISCONNECTED = 3,
        RTC_FAILED = 4,
        RTC_CLOSED = 5,
    }

    public enum rtcIceState
    {
        RTC_ICE_NEW = 0,
        RTC_ICE_CHECKING = 1,
        RTC_ICE_CONNECTED = 2,
        RTC_ICE_COMPLETED = 3,
        RTC_ICE_FAILED = 4,
        RTC_ICE_DISCONNECTED = 5,
        RTC_ICE_CLOSED = 6,
    }

    public enum rtcGatheringState
    {
        RTC_GATHERING_NEW = 0,
        RTC_GATHERING_INPROGRESS = 1,
        RTC_GATHERING_COMPLETE = 2,
    }

    public enum rtcSignalingState
    {
        RTC_SIGNALING_STABLE = 0,
        RTC_SIGNALING_HAVE_LOCAL_OFFER = 1,
        RTC_SIGNALING_HAVE_REMOTE_OFFER = 2,
        RTC_SIGNALING_HAVE_LOCAL_PRANSWER = 3,
        RTC_SIGNALING_HAVE_REMOTE_PRANSWER = 4,
    }

    public enum rtcLogLevel
    {
        RTC_LOG_NONE = 0,
        RTC_LOG_FATAL = 1,
        RTC_LOG_ERROR = 2,
        RTC_LOG_WARNING = 3,
        RTC_LOG_INFO = 4,
        RTC_LOG_DEBUG = 5,
        RTC_LOG_VERBOSE = 6,
    }

    public enum rtcCertificateType
    {
        RTC_CERTIFICATE_DEFAULT = 0,
        RTC_CERTIFICATE_ECDSA = 1,
        RTC_CERTIFICATE_RSA = 2,
    }

    public enum rtcCodec
    {
        RTC_CODEC_H264 = 0,
        RTC_CODEC_VP8 = 1,
        RTC_CODEC_VP9 = 2,
        RTC_CODEC_H265 = 3,
        RTC_CODEC_AV1 = 4,
        RTC_CODEC_OPUS = 128,
        RTC_CODEC_PCMU = 129,
        RTC_CODEC_PCMA = 130,
        RTC_CODEC_AAC = 131,
        RTC_CODEC_G722 = 132,
    }

    public enum rtcDirection
    {
        RTC_DIRECTION_UNKNOWN = 0,
        RTC_DIRECTION_SENDONLY = 1,
        RTC_DIRECTION_RECVONLY = 2,
        RTC_DIRECTION_SENDRECV = 3,
        RTC_DIRECTION_INACTIVE = 4,
    }

    public enum rtcTransportPolicy
    {
        RTC_TRANSPORT_POLICY_ALL = 0,
        RTC_TRANSPORT_POLICY_RELAY = 1,
    }

    public unsafe partial struct rtcConfiguration
    {
        [NativeTypeName("const char **")]
        public sbyte** iceServers;

        public int iceServersCount;

        [NativeTypeName("const char *")]
        public sbyte* proxyServer;

        [NativeTypeName("const char *")]
        public sbyte* bindAddress;

        public rtcCertificateType certificateType;

        public rtcTransportPolicy iceTransportPolicy;

        [NativeTypeName("bool")]
        public byte enableIceTcp;

        [NativeTypeName("bool")]
        public byte enableIceUdpMux;

        [NativeTypeName("bool")]
        public byte disableAutoNegotiation;

        [NativeTypeName("bool")]
        public byte forceMediaTransport;

        [NativeTypeName("uint16_t")]
        public ushort portRangeBegin;

        [NativeTypeName("uint16_t")]
        public ushort portRangeEnd;

        public int mtu;

        public int maxMessageSize;
    }

    public partial struct rtcReliability
    {
        [NativeTypeName("bool")]
        public byte unordered;

        [NativeTypeName("bool")]
        public byte unreliable;

        [NativeTypeName("unsigned int")]
        public uint maxPacketLifeTime;

        [NativeTypeName("unsigned int")]
        public uint maxRetransmits;
    }

    public unsafe partial struct rtcDataChannelInit
    {
        public rtcReliability reliability;

        [NativeTypeName("const char *")]
        public sbyte* protocol;

        [NativeTypeName("bool")]
        public byte negotiated;

        [NativeTypeName("bool")]
        public byte manualStream;

        [NativeTypeName("uint16_t")]
        public ushort stream;
    }

    public unsafe partial struct rtcTrackInit
    {
        public rtcDirection direction;

        public rtcCodec codec;

        public int payloadType;

        [NativeTypeName("uint32_t")]
        public uint ssrc;

        [NativeTypeName("const char *")]
        public sbyte* mid;

        [NativeTypeName("const char *")]
        public sbyte* name;

        [NativeTypeName("const char *")]
        public sbyte* msid;

        [NativeTypeName("const char *")]
        public sbyte* trackId;

        [NativeTypeName("const char *")]
        public sbyte* profile;
    }

    public enum rtcObuPacketization
    {
        RTC_OBU_PACKETIZED_OBU = 0,
        RTC_OBU_PACKETIZED_TEMPORAL_UNIT = 1,
    }

    public enum rtcNalUnitSeparator
    {
        RTC_NAL_SEPARATOR_LENGTH = 0,
        RTC_NAL_SEPARATOR_LONG_START_SEQUENCE = 1,
        RTC_NAL_SEPARATOR_SHORT_START_SEQUENCE = 2,
        RTC_NAL_SEPARATOR_START_SEQUENCE = 3,
    }

    public unsafe partial struct rtcPacketizerInit
    {
        [NativeTypeName("uint32_t")]
        public uint ssrc;

        [NativeTypeName("const char *")]
        public sbyte* cname;

        [NativeTypeName("uint8_t")]
        public byte payloadType;

        [NativeTypeName("uint32_t")]
        public uint clockRate;

        [NativeTypeName("uint16_t")]
        public ushort sequenceNumber;

        [NativeTypeName("uint32_t")]
        public uint timestamp;

        [NativeTypeName("uint16_t")]
        public ushort maxFragmentSize;

        public rtcNalUnitSeparator nalSeparator;

        public rtcObuPacketization obuPacketization;

        [NativeTypeName("uint8_t")]
        public byte playoutDelayId;

        [NativeTypeName("uint16_t")]
        public ushort playoutDelayMin;

        [NativeTypeName("uint16_t")]
        public ushort playoutDelayMax;

        [NativeTypeName("uint8_t")]
        public byte colorSpaceId;

        [NativeTypeName("uint8_t")]
        public byte colorChromaSitingHorz;

        [NativeTypeName("uint8_t")]
        public byte colorChromaSitingVert;

        [NativeTypeName("uint8_t")]
        public byte colorRange;

        [NativeTypeName("uint8_t")]
        public byte colorPrimaries;

        [NativeTypeName("uint8_t")]
        public byte colorTransfer;

        [NativeTypeName("uint8_t")]
        public byte colorMatrix;
    }

    public unsafe partial struct rtcSsrcForTypeInit
    {
        [NativeTypeName("uint32_t")]
        public uint ssrc;

        [NativeTypeName("const char *")]
        public sbyte* name;

        [NativeTypeName("const char *")]
        public sbyte* msid;

        [NativeTypeName("const char *")]
        public sbyte* trackId;
    }

    public unsafe partial struct rtcWsConfiguration
    {
        [NativeTypeName("bool")]
        public byte disableTlsVerification;

        [NativeTypeName("const char *")]
        public sbyte* proxyServer;

        [NativeTypeName("const char **")]
        public sbyte** protocols;

        public int protocolsCount;

        public int connectionTimeoutMs;

        public int pingIntervalMs;

        public int maxOutstandingPings;

        public int maxMessageSize;
    }

    public unsafe partial struct rtcWsServerConfiguration
    {
        [NativeTypeName("uint16_t")]
        public ushort port;

        [NativeTypeName("bool")]
        public byte enableTls;

        [NativeTypeName("const char *")]
        public sbyte* certificatePemFile;

        [NativeTypeName("const char *")]
        public sbyte* keyPemFile;

        [NativeTypeName("const char *")]
        public sbyte* keyPemPass;

        [NativeTypeName("const char *")]
        public sbyte* bindAddress;

        public int connectionTimeoutMs;

        public int maxMessageSize;
    }

    public partial struct rtcSctpSettings
    {
        public int recvBufferSize;

        public int sendBufferSize;

        public int maxChunksOnQueue;

        public int initialCongestionWindow;

        public int maxBurst;

        public int congestionControlModule;

        public int delayedSackTimeMs;

        public int minRetransmitTimeoutMs;

        public int maxRetransmitTimeoutMs;

        public int initialRetransmitTimeoutMs;

        public int maxRetransmitAttempts;

        public int heartbeatIntervalMs;
    }

    public static unsafe partial class Rtc
    {
        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void rtcInitLogger(rtcLogLevel level, [NativeTypeName("rtcLogCallbackFunc")] delegate* unmanaged[Cdecl]<rtcLogLevel, sbyte*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void rtcSetUserPointer(int id, void* ptr);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* rtcGetUserPointer(int i);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcCreatePeerConnection([NativeTypeName("const rtcConfiguration *")] rtcConfiguration* config);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcClosePeerConnection(int pc);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcDeletePeerConnection(int pc);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetLocalDescriptionCallback(int pc, [NativeTypeName("rtcDescriptionCallbackFunc")] delegate* unmanaged[Cdecl]<int, sbyte*, sbyte*, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetLocalCandidateCallback(int pc, [NativeTypeName("rtcCandidateCallbackFunc")] delegate* unmanaged[Cdecl]<int, sbyte*, sbyte*, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetStateChangeCallback(int pc, [NativeTypeName("rtcStateChangeCallbackFunc")] delegate* unmanaged[Cdecl]<int, rtcState, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetIceStateChangeCallback(int pc, [NativeTypeName("rtcIceStateChangeCallbackFunc")] delegate* unmanaged[Cdecl]<int, rtcIceState, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetGatheringStateChangeCallback(int pc, [NativeTypeName("rtcGatheringStateCallbackFunc")] delegate* unmanaged[Cdecl]<int, rtcGatheringState, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetSignalingStateChangeCallback(int pc, [NativeTypeName("rtcSignalingStateCallbackFunc")] delegate* unmanaged[Cdecl]<int, rtcSignalingState, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetLocalDescription(int pc, [NativeTypeName("const char *")] sbyte* type);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetRemoteDescription(int pc, [NativeTypeName("const char *")] sbyte* sdp, [NativeTypeName("const char *")] sbyte* type);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcAddRemoteCandidate(int pc, [NativeTypeName("const char *")] sbyte* cand, [NativeTypeName("const char *")] sbyte* mid);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetLocalDescription(int pc, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetRemoteDescription(int pc, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetLocalDescriptionType(int pc, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetRemoteDescriptionType(int pc, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcCreateOffer(int pc, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcCreateAnswer(int pc, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetLocalAddress(int pc, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetRemoteAddress(int pc, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetSelectedCandidatePair(int pc, [NativeTypeName("char *")] sbyte* local, int localSize, [NativeTypeName("char *")] sbyte* remote, int remoteSize);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte rtcIsNegotiationNeeded(int pc);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetMaxDataChannelStream(int pc);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetRemoteMaxMessageSize(int pc);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetOpenCallback(int id, [NativeTypeName("rtcOpenCallbackFunc")] delegate* unmanaged[Cdecl]<int, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetClosedCallback(int id, [NativeTypeName("rtcClosedCallbackFunc")] delegate* unmanaged[Cdecl]<int, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetErrorCallback(int id, [NativeTypeName("rtcErrorCallbackFunc")] delegate* unmanaged[Cdecl]<int, sbyte*, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetMessageCallback(int id, [NativeTypeName("rtcMessageCallbackFunc")] delegate* unmanaged[Cdecl]<int, sbyte*, int, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSendMessage(int id, [NativeTypeName("const char *")] sbyte* data, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcClose(int id);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcDelete(int id);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte rtcIsOpen(int id);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte rtcIsClosed(int id);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcMaxMessageSize(int id);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetBufferedAmount(int id);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetBufferedAmountLowThreshold(int id, int amount);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetBufferedAmountLowCallback(int id, [NativeTypeName("rtcBufferedAmountLowCallbackFunc")] delegate* unmanaged[Cdecl]<int, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetAvailableAmount(int id);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetAvailableCallback(int id, [NativeTypeName("rtcAvailableCallbackFunc")] delegate* unmanaged[Cdecl]<int, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcReceiveMessage(int id, [NativeTypeName("char *")] sbyte* buffer, int* size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetDataChannelCallback(int pc, [NativeTypeName("rtcDataChannelCallbackFunc")] delegate* unmanaged[Cdecl]<int, int, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcCreateDataChannel(int pc, [NativeTypeName("const char *")] sbyte* label);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcCreateDataChannelEx(int pc, [NativeTypeName("const char *")] sbyte* label, [NativeTypeName("const rtcDataChannelInit *")] rtcDataChannelInit* init);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcDeleteDataChannel(int dc);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetDataChannelStream(int dc);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetDataChannelLabel(int dc, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetDataChannelProtocol(int dc, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetDataChannelReliability(int dc, rtcReliability* reliability);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetTrackCallback(int pc, [NativeTypeName("rtcTrackCallbackFunc")] delegate* unmanaged[Cdecl]<int, int, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcAddTrack(int pc, [NativeTypeName("const char *")] sbyte* mediaDescriptionSdp);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcAddTrackEx(int pc, [NativeTypeName("const rtcTrackInit *")] rtcTrackInit* init);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcDeleteTrack(int tr);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetTrackDescription(int tr, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetTrackMid(int tr, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetTrackDirection(int tr, rtcDirection* direction);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcRequestKeyframe(int tr);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcRequestBitrate(int tr, [NativeTypeName("unsigned int")] uint bitrate);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("rtcMessage *")]
        public static extern void** rtcCreateOpaqueMessage(void* data, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void rtcDeleteOpaqueMessage([NativeTypeName("rtcMessage *")] void** msg);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetMediaInterceptorCallback(int id, [NativeTypeName("rtcInterceptorCallbackFunc")] delegate* unmanaged[Cdecl]<int, sbyte*, int, void*, void*> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetH264Packetizer(int tr, [NativeTypeName("const rtcPacketizerInit *")] rtcPacketizerInit* init);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetH265Packetizer(int tr, [NativeTypeName("const rtcPacketizerInit *")] rtcPacketizerInit* init);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetAV1Packetizer(int tr, [NativeTypeName("const rtcPacketizerInit *")] rtcPacketizerInit* init);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetOpusPacketizer(int tr, [NativeTypeName("const rtcPacketizerInit *")] rtcPacketizerInit* init);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetAACPacketizer(int tr, [NativeTypeName("const rtcPacketizerInit *")] rtcPacketizerInit* init);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetPCMUPacketizer(int tr, [NativeTypeName("const rtcPacketizerInit *")] rtcPacketizerInit* init);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetPCMAPacketizer(int tr, [NativeTypeName("const rtcPacketizerInit *")] rtcPacketizerInit* init);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetG722Packetizer(int tr, [NativeTypeName("const rtcPacketizerInit *")] rtcPacketizerInit* init);

        [Obsolete]
        public static int rtcSetH264PacketizationHandler(int tr, [NativeTypeName("const rtcPacketizationHandlerInit *")] rtcPacketizerInit* init)
        {
            return rtcSetH264Packetizer(tr, init);
        }

        [Obsolete]
        public static int rtcSetH265PacketizationHandler(int tr, [NativeTypeName("const rtcPacketizationHandlerInit *")] rtcPacketizerInit* init)
        {
            return rtcSetH265Packetizer(tr, init);
        }

        [Obsolete]
        public static int rtcSetAV1PacketizationHandler(int tr, [NativeTypeName("const rtcPacketizationHandlerInit *")] rtcPacketizerInit* init)
        {
            return rtcSetAV1Packetizer(tr, init);
        }

        [Obsolete]
        public static int rtcSetOpusPacketizationHandler(int tr, [NativeTypeName("const rtcPacketizationHandlerInit *")] rtcPacketizerInit* init)
        {
            return rtcSetOpusPacketizer(tr, init);
        }

        [Obsolete]
        public static int rtcSetAACPacketizationHandler(int tr, [NativeTypeName("const rtcPacketizationHandlerInit *")] rtcPacketizerInit* init)
        {
            return rtcSetAACPacketizer(tr, init);
        }

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcChainRtcpReceivingSession(int tr);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcChainRtcpSrReporter(int tr);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcChainRtcpNackResponder(int tr, [NativeTypeName("unsigned int")] uint maxStoredPacketsCount);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcChainPliHandler(int tr, [NativeTypeName("rtcPliHandlerCallbackFunc")] delegate* unmanaged[Cdecl]<int, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcChainRembHandler(int tr, [NativeTypeName("rtcRembHandlerCallbackFunc")] delegate* unmanaged[Cdecl]<int, uint, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcTransformSecondsToTimestamp(int id, double seconds, [NativeTypeName("uint32_t *")] uint* timestamp);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcTransformTimestampToSeconds(int id, [NativeTypeName("uint32_t")] uint timestamp, double* seconds);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetCurrentTrackTimestamp(int id, [NativeTypeName("uint32_t *")] uint* timestamp);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetTrackRtpTimestamp(int id, [NativeTypeName("uint32_t")] uint timestamp);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetLastTrackSenderReportTimestamp(int id, [NativeTypeName("uint32_t *")] uint* timestamp);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetTrackPayloadTypesForCodec(int tr, [NativeTypeName("const char *")] sbyte* ccodec, int* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetSsrcsForTrack(int tr, [NativeTypeName("uint32_t *")] uint* buffer, int count);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetCNameForSsrc(int tr, [NativeTypeName("uint32_t")] uint ssrc, [NativeTypeName("char *")] sbyte* cname, int cnameSize);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetSsrcsForType([NativeTypeName("const char *")] sbyte* mediaType, [NativeTypeName("const char *")] sbyte* sdp, [NativeTypeName("uint32_t *")] uint* buffer, int bufferSize);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetSsrcForType([NativeTypeName("const char *")] sbyte* mediaType, [NativeTypeName("const char *")] sbyte* sdp, [NativeTypeName("char *")] sbyte* buffer, [NativeTypeName("const int")] int bufferSize, rtcSsrcForTypeInit* init);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [Obsolete]
        public static extern int rtcSetNeedsToSendRtcpSr(int id);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcCreateWebSocket([NativeTypeName("const char *")] sbyte* url);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcCreateWebSocketEx([NativeTypeName("const char *")] sbyte* url, [NativeTypeName("const rtcWsConfiguration *")] rtcWsConfiguration* config);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcDeleteWebSocket(int ws);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetWebSocketRemoteAddress(int ws, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetWebSocketPath(int ws, [NativeTypeName("char *")] sbyte* buffer, int size);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcCreateWebSocketServer([NativeTypeName("const rtcWsServerConfiguration *")] rtcWsServerConfiguration* config, [NativeTypeName("rtcWebSocketClientCallbackFunc")] delegate* unmanaged[Cdecl]<int, int, void*, void> cb);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcDeleteWebSocketServer(int wsserver);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcGetWebSocketServerPort(int wsserver);

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void rtcPreload();

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void rtcCleanup();

        [DllImport("datachannel", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int rtcSetSctpSettings([NativeTypeName("const rtcSctpSettings *")] rtcSctpSettings* settings);

        [NativeTypeName("#define RTC_ENABLE_WEBSOCKET 1")]
        public const int RTC_ENABLE_WEBSOCKET = 1;

        [NativeTypeName("#define RTC_ENABLE_MEDIA 1")]
        public const int RTC_ENABLE_MEDIA = 1;

        [NativeTypeName("#define RTC_DEFAULT_MTU 1280")]
        public const int RTC_DEFAULT_MTU = 1280;

        [NativeTypeName("#define RTC_DEFAULT_MAX_FRAGMENT_SIZE ((uint16_t)(RTC_DEFAULT_MTU - 12 - 8 - 40))")]
        public const ushort RTC_DEFAULT_MAX_FRAGMENT_SIZE = ((ushort)(1280 - 12 - 8 - 40));

        [NativeTypeName("#define RTC_DEFAULT_MAX_STORED_PACKET_COUNT 512")]
        public const int RTC_DEFAULT_MAX_STORED_PACKET_COUNT = 512;

        [NativeTypeName("#define RTC_DEFAULT_MAXIMUM_FRAGMENT_SIZE RTC_DEFAULT_MAX_FRAGMENT_SIZE")]
        public const ushort RTC_DEFAULT_MAXIMUM_FRAGMENT_SIZE = ((ushort)(1280 - 12 - 8 - 40));

        [NativeTypeName("#define RTC_DEFAULT_MAXIMUM_PACKET_COUNT_FOR_NACK_CACHE RTC_DEFAULT_MAX_STORED_PACKET_COUNT")]
        public const int RTC_DEFAULT_MAXIMUM_PACKET_COUNT_FOR_NACK_CACHE = 512;

        [NativeTypeName("#define RTC_ERR_SUCCESS 0")]
        public const int RTC_ERR_SUCCESS = 0;

        [NativeTypeName("#define RTC_ERR_INVALID -1")]
        public const int RTC_ERR_INVALID = -1;

        [NativeTypeName("#define RTC_ERR_FAILURE -2")]
        public const int RTC_ERR_FAILURE = -2;

        [NativeTypeName("#define RTC_ERR_NOT_AVAIL -3")]
        public const int RTC_ERR_NOT_AVAIL = -3;

        [NativeTypeName("#define RTC_ERR_TOO_SMALL -4")]
        public const int RTC_ERR_TOO_SMALL = -4;
    }
}
