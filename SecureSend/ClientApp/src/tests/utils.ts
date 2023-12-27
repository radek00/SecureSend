export const waitForExpect = async (callback: () => any, waitFor = 100) => {
  return new Promise((resolve, reject) => {
    setTimeout(() => {
      try {
        resolve(callback());
      } catch (err) {
        reject(err);
      }
    }, waitFor);
  });
};
