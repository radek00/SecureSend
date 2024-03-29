const baseUrl = import.meta.env.BASE_URL;
const endpoints = {
  secureSend: `${baseUrl}api/SecureSend`,
  uploadChunks: `${baseUrl}api/SecureSend/uploadChunks`,
  download: `${baseUrl}api/SecureSend/download`,
  cancelUpload: `${baseUrl}api/SecureSend/cancelUpload`,
  uploadLimits: `${baseUrl}api/uploadLimits`,
};

export default endpoints;
