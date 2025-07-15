﻿namespace RiskTrackLoginApi.Constracts
{
    public record AuthenticationCodeGenerated
    {
        public string Email { get; set; }=string.Empty;
        public string Code {  get; set; }=string.Empty;
    }
}
