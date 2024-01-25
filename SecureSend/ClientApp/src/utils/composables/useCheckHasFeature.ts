export function useCheckHasFeature(featureName: string) {
    if(featureName in window) return true;
    return false;
}