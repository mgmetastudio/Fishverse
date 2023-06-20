using Firebase;
using Firebase.Database;
using LibEngine.Auth;
using System.Net.Http;
using Zenject;

public class LibEngineInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<HttpClient>().FromMethod(GetHttpClient).AsSingle();

        SetupAuth();
    }

    private void SetupAuth()
    {
        var dataAuthUserState = new PlayerPrefsSaveableScheme<AuthUserState>("AuthUserState0");
        Container.Bind<IAuthUserState>().FromInstance(dataAuthUserState.Current).AsSingle();
        Container.Bind<ISaveable<AuthUserState>>().FromInstance(dataAuthUserState).AsSingle();

        Container.Bind<IAuthValidateService>().To<AuthValidateService>().AsSingle(); //API PROVIDER

        // Firebase
        Container.Bind<FirebaseApp>().FromMethod(GetFirebaseApp).AsSingle();

        Container.BindInterfacesAndSelfTo<AuthSuccessedContinueFlowStrategy>().AsSingle(); //todo: create event

        Container.Bind<IAuthProvider>().To<AuthApiProvider>().AsSingle(); //API PROVIDER
                                                                         //Container.Bind<IAuthProvider>().To<AuthProviderFirebase>().AsSingle(); //FIREBASE PROVIDER

        // AuthManager
        Container.Bind<IAuthManager>().To<AuthManager>().AsSingle();

        Container.Bind<DatabaseReference>().FromMethod(GetFirebaseAppDb).AsSingle();

        Container.Bind<IProfileStatsController>().To<ProfileStatsController>().AsSingle(); //client?
    }

    //Helpers getters
    private HttpClient httpClient = new HttpClient(); //todo: factory
    private HttpClient GetHttpClient()
    {
        return httpClient;
    }

    private FirebaseApp GetFirebaseApp()
    {
        return FirebaseApp.DefaultInstance;
    }

    private DatabaseReference GetFirebaseAppDb()
    {
        return FirebaseDatabase.DefaultInstance.RootReference;
    }
}
