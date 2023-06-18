const baseUrl = import.meta.env.BASE_URL;
const endpoints =  {
    secureSend: `${baseUrl}api/SecureSend`,
    uploadChunks: `${baseUrl}api/SecureSend/uploadChunks`,

}

export default endpoints;